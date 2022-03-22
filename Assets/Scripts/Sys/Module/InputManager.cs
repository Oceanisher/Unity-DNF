using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sys.Config;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Sys.Module
{
    //输入中枢
    public class InputManager : AbstractManager<InputManager>, IKeyHandler
    {
        #region 事件委托

        //按键回调
        public UnityAction<List<KeyInfo>> OnKeyEvent;

        #endregion

        [Header("默认按键配置")] [SerializeField] 
        private InputSo defaultInputSo;

        [Header("自定义按键配置")] [SerializeField] [CanBeNull]
        private InputSo customInputSo;

        [Header("双击按键判断最长间隔,MS")] [SerializeField]
        private float doubleKeyInterval;
        
        //按键记录
        private Dictionary<KeyCode, KeyInfo> _keyRecord = new Dictionary<KeyCode, KeyInfo>();

        //按键临时存储空间
        private Dictionary<KeyCode, KeyTemp> _frameKeyRecord = new Dictionary<KeyCode, KeyTemp>();
        
        //每帧最终按键记录
        private List<KeyInfo> _finalFrameKeyInfos = new List<KeyInfo>();
        
        public override void Init(SceneType type)
        {
            //TODO
            base.Init(type);

            HasInit = true;
        }

        private void Update()
        {
            //按键监听
            //TODO 单击持续与双击持续还可以优化
            OnKey();
        }

        private void OnKey()
        {
            //清空列表
            ClearTempData();
            //按键获取
            GetKeyInput();

            //按键过滤
            FilterKey();

            //判断该帧是否有按键需要处理
            if (!FrameHasKeyToHandle())
            {
                return;
            }
            
            // foreach (var item in _finalFrameKeyInfos)
            // {
            //     if (item.KeyType == KeyType.Jump)
            //     {
            //         Log.Info($"[Input]按键:{item.KeyCode}, 类型:{item.KeyState}", LogModule.Manager);
            //     }
            // }
            
            //发送事件
            SendKeyEvent();
        }

        //过滤按键
        private void FilterKey()
        {
            //处理新按键
            foreach (var item in _frameKeyRecord)
            {
                //如果历史列表不包含该按键，说明是新的、单击的
                if (!_keyRecord.ContainsKey(item.Key))
                {
                    KeyInfo keyInfo = KeyInfo.Build(item.Key, GameManager.Instance.FrameTimestamp, GameManager.Instance.FrameCount,
                        item.Value.IsHold ? KeyState.SingleHold : KeyState.Single, item.Value.KeyType);
                    _keyRecord.Add(keyInfo.KeyCode, keyInfo);
                    _finalFrameKeyInfos.Add(keyInfo);
                }
                //如果包含，那么要判断是什么行为
                else
                {
                    _keyRecord.TryGetValue(item.Key, out var recordKey);
                    //如果上一个按键的间隔已经超过指定间隔，那么相当于失效，重置为新按键；
                    //否则，根据前一个按键状态判断新的状态
                    KeyState newState = IsKeyTimeout(recordKey) ?
                        (item.Value.IsHold ? KeyState.SingleHold : KeyState.Single) 
                        : GetCurrentStateCompareToPre(recordKey.KeyState, item.Value.IsHold);
                    UpdateKeyInfo(recordKey, newState);
                    _finalFrameKeyInfos.Add(recordKey);
                }
            }

            //历史Hold清除
            //现存按键中Hold按键数量
            foreach (var item in _keyRecord)
            {
                //非Hold跳过
                if (!KeyStateExtend.IsHold(item.Value.KeyState))
                {
                    continue;
                }
                //超时的跳过
                if (IsKeyTimeout(item.Value))
                {
                    continue;
                }
                //当前帧处理过的跳过
                if (IsKeyCurrentFrame(item.Value))
                {
                    continue;
                }
                //Hold按键，如果当前帧已经没有该按键的Hold，那么就转换为Release
                if (!_frameKeyRecord.ContainsKey(item.Key))
                {
                    UpdateKeyInfo(item.Value, 
                        item.Value.KeyState == KeyState.SingleHold ? KeyState.SingleHoldRelease : KeyState.DoubleHoldRelease);
                    _finalFrameKeyInfos.Add(item.Value);
                }
            }
        }

        //获取按键输入
        //bool：true是按住行为，false是按下行为
        private void GetKeyInput()
        {
            Dictionary<KeyCode, KeyType> activeKeyMap = GetActiveKeyMap();
            // List<KeyCode> activeKeyList = GetActiveKeyList();
            _frameKeyRecord.Clear();

            //判断有按下、或者按住
            if (Input.anyKey || Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    //如果不是有效按键，排除
                    if (!activeKeyMap.ContainsKey(key))
                    {
                        continue;
                    }

                    //按下、按住同时发生，所以当有按下时，优先判断按下
                    //按住的按键
                    if (Input.GetKeyDown(key))
                    {
                        activeKeyMap.TryGetValue(key, out var keyType);
                        KeyTemp keyTemp = KeyTemp.Build(keyType, false);
                        _frameKeyRecord.Add(key, keyTemp);
                    }
                    //按下的按键
                    else if (Input.GetKey(key))
                    {
                        activeKeyMap.TryGetValue(key, out var keyType);
                        KeyTemp keyTemp = KeyTemp.Build(keyType, true);
                        _frameKeyRecord.Add(key, keyTemp);
                    }
                }
            }
        }

        #region 工具方法

        //清空每帧临时数据
        private void ClearTempData()
        {
            _frameKeyRecord.Clear();
            _finalFrameKeyInfos.Clear();
        }

        //本帧是否有需要处理的按键
        private bool FrameHasKeyToHandle()
        {
            return _finalFrameKeyInfos.Count != 0;
        }

        //更新KeyInfo
        private void UpdateKeyInfo(KeyInfo keyInfo, KeyState keyState)
        {
            keyInfo.KeyState = keyState;
            keyInfo.FrameTime = GameManager.Instance.FrameTimestamp;
            keyInfo.FrameCount = GameManager.Instance.FrameCount;
        }

        //按键是否超时-两次按键之间的间隔是否超时
        private bool IsKeyTimeout(KeyInfo keyInfo)
        {
            return GameManager.Instance.FrameTimestamp - keyInfo.FrameTime > doubleKeyInterval;
        }

        //按键是否是当前帧处理过的
        private bool IsKeyCurrentFrame(KeyInfo keyInfo)
        {
            return keyInfo.FrameCount == GameManager.Instance.FrameCount && (keyInfo.FrameTime - GameManager.Instance.FrameTimestamp) < float.Epsilon;
        }

        //获取生效的按键映射
        private Dictionary<KeyCode, KeyType> GetActiveKeyMap()
        {
            InputSo inputSo = GetActiveInputSo();
            return inputSo.GetKeyMapByScene(GameManager.Instance.SceneType);
        }

        //获取有效的输入配置
        private InputSo GetActiveInputSo()
        {
            if (null != customInputSo)
            {
                return customInputSo;
            }

            return defaultInputSo;
        }

        //根据上次按键行为获取当前按键行为
        private KeyState GetCurrentStateCompareToPre(KeyState preState, bool isHold)
        {
            //如果按键是hold
            if (isHold)
            {
                //如果上一个按键是Hold，那么不变，更新其他信息
                if (preState == KeyState.SingleHold || preState == KeyState.DoubleHold)
                {
                    return preState;
                }
                //如果上一个是单击，那么变成单击Hold
                if (preState == KeyState.Single)
                {
                    return KeyState.SingleHold;
                }
                //如果上一个是双击，那么变成双击Hold
                if (preState == KeyState.Double)
                {
                    return KeyState.DoubleHold;
                }
                //否则的话其实应该是错误按键，因为HoldRelease之后应该先是Down才能是Hold，这里不处理。
            }
            //如果按键是单击
            else
            {
                //如果上一个按键是单击，那么变成双击
                if (preState == KeyState.Single)
                {
                    return KeyState.Double;
                }
                //如果上一个是单击Hold，那么变成双击
                if (preState == KeyState.SingleHold)
                {
                    return KeyState.Double;
                }
                //如果上一个是双击，那么变成双击Hold
                if (preState == KeyState.Double)
                {
                    return KeyState.DoubleHold;
                }
                //如果上一个按键是单击release，那么变成双击
                if (preState == KeyState.SingleHoldRelease)
                {
                    return KeyState.Double;
                }
                //如果上一个按键是双击release，那么不过时的情况下仍旧是双击
                if (preState == KeyState.DoubleHoldRelease)
                {
                    return KeyState.Double;
                }
                //否则的话其实应该是错误按键，因为Hold之后应该先是Release才能是单击，这里不处理。
            }
            return KeyState.None;
        }

        #endregion

        #region 事件发出

        //发送按键事件
        private void SendKeyEvent()
        {
            OnKeyEvent?.Invoke(_finalFrameKeyInfos);
        }

        #endregion
        
        #region IKeyHanlder 接口

        public void HandleKeys(List<InputManager.KeyInfo> keyInfos)
        {
            //无需处理
        }

        #endregion

        #region 内部工具类

        //按键信息
        public class KeyInfo
        {
            //按键
            public KeyCode KeyCode;
            
            //按键状态
            public KeyState KeyState;

            //按键类型
            public KeyType KeyType;

            //按键时间，毫秒
            public float FrameTime;

            //帧标记
            public int FrameCount;

            //构建空对象
            private static KeyInfo BuildNone(float frameTime, int frameCount)
            {
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.KeyCode = KeyCode.None;
                keyInfo.FrameTime = frameTime;
                keyInfo.FrameCount = frameCount;
                keyInfo.KeyState = KeyState.None;
                keyInfo.KeyType = KeyType.None;
                return keyInfo;
            }

            //构建普通对象
            public static KeyInfo Build(KeyCode keyCode, float frameTime, int frameCount, KeyState keyState, KeyType keyType)
            {
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.KeyCode = keyCode;
                keyInfo.FrameTime = frameTime;
                keyInfo.FrameCount = frameCount;
                keyInfo.KeyState = keyState;
                keyInfo.KeyType = keyType;
                return keyInfo;
            }
        }

        //按键临时信息
        private class KeyTemp
        {
            //按键类型
            public KeyType KeyType;
            //是否是Hold行为
            public bool IsHold;

            //构建
            public static KeyTemp Build(KeyType keyType, bool isHold)
            {
                KeyTemp keyTemp = new KeyTemp();
                keyTemp.KeyType = keyType;
                keyTemp.IsHold = isHold;
                return keyTemp;
            }
        }
        
        #endregion
    }
}