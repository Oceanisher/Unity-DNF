using System;
using System.Collections.Generic;
using Sys.Config;
using Tools;
using UnityEngine;

namespace Sys.Module.UI
{
    //HUD中枢
    public class HudManager : AbstractManager<HudManager>, IKeyHandler
    {
        //测试行为Bar
        private TestActionBar _testActionBar;

        //是否是Hub控制行为
        [HideInInspector]
        public bool isActionHubControl;
        
        public override void Init(SceneType type)
        {
            base.Init(type);

            _testActionBar = transform.Find(GameConst.HudPath_TestActionBarPath).GetComponent<TestActionBar>();
            _testActionBar.Init(MapManager.Instance.PlayerCore);

            HasInit = true;
        }

        //处理按键
        private void HandleKeysInner(List<InputManager.KeyInfo> keyInfos)
        {
            //TODO 目前根据F1的按键决定是否展示TestActionBar
            if (CollectionUtil.IsEmpty(keyInfos))
            {
                return;
            }

            foreach (var item in keyInfos)
            {
                if (item.KeyType == KeyType.F1 && item.KeyState == KeyState.Single)
                {
                    ToggleTestActionBar();
                }
            }
        }

        //隐藏/展示测试行为Bar
        private void ToggleTestActionBar()
        {
            _testActionBar.gameObject.SetActive(!_testActionBar.gameObject.activeSelf);
        }

        #region IKeyHanlder 接口

        public void HandleKeys(List<InputManager.KeyInfo> keyInfos)
        {
            HandleKeysInner(keyInfos);
        }
        
        #endregion
    }
}