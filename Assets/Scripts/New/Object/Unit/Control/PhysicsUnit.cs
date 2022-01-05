using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using UnityEngine;

namespace New.Object.Unit.Control
{
    //物理单元
    public class PhysicsUnit : AbstractObjUnit
    {
        #region AbstractObjUnit 接口

        public override void Init(ObjCore core)
        {
            base.Init(core);
            HasInit = true;
        }

        #endregion
    }
}