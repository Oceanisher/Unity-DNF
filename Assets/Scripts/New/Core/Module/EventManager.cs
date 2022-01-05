using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Tools;
using UnityEngine;

namespace New.Core.Module
{
    //事件中枢
    public class EventManager : AbstractManager<EventManager>, IKeyHandler
    {
        public override void Init(SceneType type)
        {
            //TODO
            base.Init(type);
        }

        #region IKeyHanlder 接口

        public void HandleKeys(List<InputManager.KeyInfo> keyInfos)
        {
            //TODO 
        }

        #endregion
    }
}