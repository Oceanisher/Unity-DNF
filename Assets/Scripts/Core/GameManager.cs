using System;
using System.Collections;
using System.Collections.Generic;
using Control;
using Tools;
using UnityEngine;

namespace Core
{
    //游戏核心管理器
    [Obsolete]
    public class GameManager : MonoSingleton<GameManager>
    {
        //游戏状态，默认城镇中状态
        private SceneType _sceneType = SceneType.Town;
        public SceneType SceneType => _sceneType;

        //按键监听管理器
        private InputManager _inputManager;

        //玩家角色
        [Header("玩家角色")]
        [SerializeField]
        private GameObject _player;
        //玩家角色控制器
        private ObjSupervisor _playerSupervisor;

        //活跃物体列表
        private List<GameObject> _activeGoList;
        
        void Start()
        {
            _inputManager = InputManager.Instance;
            _playerSupervisor = _player.GetComponent<ObjSupervisor>();
            AddListener();
        }

        //添加监听器
        private void AddListener()
        {
            _inputManager.KeyDown += OnKeyDown;
        }

        //处理按键监听回调
        private void OnKeyDown(KeyInfo keyInfo)
        {
            keyInfo.BehaviourType = KeyBehaviourMap.Convert(keyInfo.KeyType);
            keyInfo.IsForce = false;
            _playerSupervisor.ChangeBehaviour(keyInfo);
        }
    }
}
