using System;
using System.Collections.Generic;
using Obj.Config.Action.Structure;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Obj.Unit.Control.Item
{
    //物理碰撞器单元项
    //主要负责监听碰撞回调
    //属于PhysicsUnit的附属，附加到实际拥有碰撞器的物体上
    //只有碰撞物体中节点包含PhysicsColUnit组件，才认为是正常碰撞
    //TODO 仅关注BoxCollider2D
    public class PhysicsColUnitItem : MonoBehaviour, IInit
    {
        //是否完成初始化
        public bool HasInit { get; set; }

        //父节点PhysicsUnit
        private PhysicsUnit _physicsUnit;
        //隶属的物理配置
        private Phy _phy;
        //隶属的物理碰撞器配置
        private PhyCol _phyCol;
        
        public PhysicsUnit PhysicsUnit => _physicsUnit;
        public Phy Phy => _phy;
        public PhyCol PhyCol => _phyCol;

        public void Init(PhysicsUnit unit, Phy phy, PhyCol phyCol)
        {
            _physicsUnit = unit;
            _phy = phy;
            _phyCol = phyCol;

            HasInit = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!HasInit || !IsActiveCol(other, out var otherColUnit))
            {
                return;
            }
            
            _physicsUnit.OnCol(this, otherColUnit, ColOccurType.Enter);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!HasInit || !IsActiveCol(other, out var otherColUnit))
            {
                return;
            }
            _physicsUnit.OnCol(this, otherColUnit, ColOccurType.Stay);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!HasInit || !IsActiveCol(other, out var otherColUnit))
            {
                return;
            }
            _physicsUnit.OnCol(this, otherColUnit, ColOccurType.Exit);
        }

        //是否是有效的碰撞器
        private bool IsActiveCol(Collider2D col, out PhysicsColUnitItem otherColUnitItem)
        {
            PhysicsColUnitItem unitItem = col.gameObject.GetComponent<PhysicsColUnitItem>();
            otherColUnitItem = unitItem;

            //有PhysicsColUnit组件+碰撞器位置相同（都是XZ、或都是Y）
            if (null == unitItem || !(col is BoxCollider2D) || _phyCol.colPos != unitItem.PhyCol.colPos)
            {
                return false;
            }
            
            //满足碰撞检测条件，即Hit->Body有效、Judge->Body
            PhyJudgeType selfType = _phy.judgeType;
            PhyJudgeType otherType = unitItem.Phy.judgeType;

            return selfType == PhyJudgeType.Body && (otherType == PhyJudgeType.Hit || otherType == PhyJudgeType.Judge);
        }
    }
}