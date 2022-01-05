using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object;
using New.Tools;
using UnityEngine;

namespace New.Core.Module
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

        //临时-玩家核心
        private ObjCore _playerCore;
        
        public override void Init(SceneType type)
        {
            //TODO 临时使用测试场景，重置角色位置、初始化角色
            if (null == player)
            {
                return;
            }

            _playerCore = player.GetComponent<ObjCore>();
            player.transform.position = Vector3.zero;
            _playerCore.Init();

            //TODO
            base.Init(type);
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