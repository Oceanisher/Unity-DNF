using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    //输入监听
    public class InputManager : MonoSingleton<InputManager>
    {
        [Header("默认输入监听配置")]
        [SerializeField]
        private InputConfigSo defaultInputSO;

        //按键事件
        public UnityAction<KeyInfo> KeyDown;

        //游戏核心管理器
        private GameManager _gm;

        //上次按键
        private KeyCode _lastKey;
        
        //上次按键时间
        private float _lastKeyTime;
        
        private void Start()
        {
            _lastKey = KeyCode.None;
            _lastKeyTime = 0;
            _gm = GameManager.Instance;
        }

        private void Update()
        {
            //TODO 用户自定义按键
            if (null == defaultInputSO)
            {
                LogUtil.Error("[InputManager]默认按键配置缺失。");
                return;
            }

            //按键监听
            OnKey();
        }

        //按键监听
        private void OnKey()
        {
            KeyCode key = GetKeyDown();

            if (key == KeyCode.None)
            {
                return;
            }

            KeyType type = KeyType.None;
            
            //根据场景判断按键是否有效
            switch (_gm.SceneType)
            {
                case SceneType.None:
                    return;
                case SceneType.Town:
                    type = defaultInputSO.GetKeyInTown(key);
                    break;
                case SceneType.Fight:
                    type = defaultInputSO.GetKeyInFight(key);
                    break;
            }
            
            if (type == KeyType.None)
            {
                return;
            }

            KeyInfo info = BuildInfo(key, type);
            
            //监听调用
            // LogUtil.Info($"[InputManager]按键监听，按键:{key},类型:{type}");
            KeyDown?.Invoke(info);
        }

        //获取按键输入
        private KeyCode GetKeyDown()
        {
            if (Input.anyKey || Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(key))
                    {
                        return key;
                    }
                }
            }
            return KeyCode.None;
        }

        //构建KeyInfo
        private KeyInfo BuildInfo(KeyCode key, KeyType type)
        {
            KeyInfo info = new KeyInfo();
            info.KeyCode = key;
            info.KeyType = type;

            //当前毫秒数
            float timeNow = Time.time * 1000;
            
            //判断是否是持续按键
            info.IsSustain = _lastKey == key && timeNow - _lastKeyTime <= CoreConstant.SustainKey;
            _lastKey = key;
            _lastKeyTime = timeNow;

            info.Time = timeNow;
            
            return info;
        }
    }
}