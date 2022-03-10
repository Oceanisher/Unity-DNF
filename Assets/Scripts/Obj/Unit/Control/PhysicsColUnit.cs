using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Obj.Unit.Control
{
    //物理碰撞器单元
    //属于PhysicsUnit的附属，附加到实际拥有碰撞器的物体上
    public class PhysicsColUnit : MonoBehaviour
    {
        [SerializeField]
        private string colName;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActiveCollider(other))
            {
                return;
            }
            Log.Info($"[PhysicsCol]碰撞进入{colName}。", LogModule.ObjCore);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsActiveCollider(other))
            {
                return;
            }
            Log.Info($"[PhysicsCol]碰撞保持{colName}。", LogModule.ObjCore);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsActiveCollider(other))
            {
                return;
            }
            Log.Info($"[PhysicsCol]碰撞结束{colName}。", LogModule.ObjCore);
        }

        //是否是有效的碰撞器
        private bool IsActiveCollider(Collider2D col)
        {
            PhysicsColUnit unit = col.gameObject.GetComponent<PhysicsColUnit>();
            return null != unit;
        }
    }
}