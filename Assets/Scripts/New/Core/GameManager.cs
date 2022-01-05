using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using New.Core.Config;
using New.Core.Module;
using Tools;
using UnityEngine;
using InputManager = New.Core.Module.InputManager;

namespace New.Core
{
    //游戏核心管理器
    public class GameManager : MonoSingleton<GameManager>
    {
        //TODO 游戏场景，默认为城镇类型。应该是MapManager发送过来的。
        private SceneType _sceneType = SceneType.Town;
        public SceneType SceneType => _sceneType;
        
        //帧标记
        private int _frameCount;
        public int FrameCount => _frameCount;
        //帧时间戳，用于记录按键时间
        private float _frameTimestamp;
        public float FrameTimestamp => _frameTimestamp;
        
        //输入中枢
        private InputManager _inputManager;
        //地图中枢
        private MapManager _mapManager;
        //事件中枢
        private EventManager _eventManager;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            //帧标记更新
            FrameFlagUpdate();
        }

        //初始化GameManager
        private void Init()
        {
            DontDestroyOnLoad(gameObject);
            
            _inputManager = GetComponent<InputManager>();
            _mapManager = GetComponent<MapManager>();
            _eventManager = GetComponent<EventManager>();

            _inputManager.Init(_sceneType);
            _mapManager.Init(_sceneType);
            _eventManager.Init(_sceneType);

            _inputManager.OnKeyEvent += OnKeyEvent;
        }

        #region 事件处理

        //按键事件
        private void OnKeyEvent(List<InputManager.KeyInfo> keyInfos)
        {
            _mapManager.HandleKeys(keyInfos);
            _eventManager.HandleKeys(keyInfos);
        }

        #endregion

        #region 工具方法-写

        //帧标记更新
        private void FrameFlagUpdate()
        {
            _frameCount = (++_frameCount) % 10000;
            _frameTimestamp = TimeUtil.TimeMs();
        }

        #endregion
    }
}