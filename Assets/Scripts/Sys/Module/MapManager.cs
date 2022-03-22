using System.Collections;
using System.Collections.Generic;
using Obj;
using Sys.Config;
using Tools;
using UnityEngine;

namespace Sys.Module
{
    //地图中枢
    public class MapManager : AbstractManager<MapManager>, IKeyHandler
    {
        [Header("测试场景")]
        [SerializeField]
        private GameObject testScene;

        [Header("玩家角色")]
        [SerializeField]
        private GameObject player;
        
        [Header("沙袋")]
        [SerializeField]
        private GameObject sandBag;

        //临时-玩家核心
        private ObjCore _playerCore;
        public ObjCore PlayerCore => _playerCore;
        //临时-沙袋核心
        private ObjCore _sandBagCore;
        public ObjCore SandBagCore => _sandBagCore;
        
        public override void Init(SceneType type)
        {
            //TODO 临时使用测试场景，重置角色位置、初始化角色
            if (null == player)
            {
                return;
            }

            _playerCore = player.GetComponent<ObjCore>();
            _playerCore.Init();
            _sandBagCore = sandBag.GetComponent<ObjCore>();
            _sandBagCore.Init();

            //TODO
            base.Init(type);
            
            HasInit = true;
        }
        
        #region IKeyHanlder 接口

        public void HandleKeys(List<InputManager.KeyInfo> keyInfos)
        {
            //TODO 
            _playerCore.ReceiveKeyInfos(keyInfos);
        }

        #endregion
    }
}