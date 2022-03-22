using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sys.Config;
using Sys.Module;
using Sys.Module.UI;
using Tools;
using UnityEngine;

namespace Sys
{
    //游戏核心管理器
    public class GameManager : MonoSingleton<GameManager>, IInit
    {
        //是否初始化完成
        public bool HasInit { set;get; }
        
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
        //音频中枢
        private AudioManager _audioManager;
        //全局物料管理器
        private GlobalResManager _globalResManager;
        //HUD中枢
        private HudManager _hudManager;

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
            _audioManager = GetComponent<AudioManager>();
            _globalResManager = GetComponent<GlobalResManager>();
            _hudManager = GetComponent<HudManager>();
            
            _inputManager.Init(_sceneType);
            _mapManager.Init(_sceneType);
            _eventManager.Init(_sceneType);
            _audioManager.Init(_sceneType);
            _globalResManager.Init(_sceneType);
            _hudManager.Init(_sceneType);

            _inputManager.OnKeyEvent += OnKeyEvent;

            HasInit = true;
        }

        #region 事件处理

        //按键事件
        private void OnKeyEvent(List<InputManager.KeyInfo> keyInfos)
        {
            _mapManager.HandleKeys(keyInfos);
            _eventManager.HandleKeys(keyInfos);
            _hudManager.HandleKeys(keyInfos);
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