using System.Collections;
using System.Collections.Generic;
using Sys.Config;
using Tools;
using UnityEngine;

namespace Sys.Module
{
    //事件中枢
    public class EventManager : AbstractManager<EventManager>
    {
        public override void Init(SceneType type)
        {
            //TODO
            base.Init(type);

            HasInit = true;
        }

        #region IKeyHanlder 接口

        public void HandleKeys(List<InputManager.KeyInfo> keyInfos)
        {
            //TODO 
        }

        #endregion
    }
}