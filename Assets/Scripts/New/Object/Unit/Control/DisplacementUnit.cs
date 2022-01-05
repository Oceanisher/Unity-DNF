using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using UnityEngine;

namespace New.Object.Unit.Control
{
    //位移单元
    public class DisplacementUnit : AbstractObjUnit
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