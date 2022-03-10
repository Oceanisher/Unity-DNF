using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obj.Config.Action;
using Obj.Config.Action.Structure;
using Obj.Event;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Obj.Unit.Control
{
    //物理单元
    public class PhysicsUnit : AbstractObjUnit
    {
        //重设碰撞器之后的回调
        public UnityAction<PhysicsInfo> OnReColPost;

        //动态碰撞器父节点-XZ轴
        private GameObject _dynamicXZ;
        public GameObject DynamicXZ => _dynamicXZ;
        //动态碰撞器父节点-Y轴
        private GameObject _dynamicY;
        public GameObject DynamicY => _dynamicY;
        //静态碰撞器父节点-XZ轴
        private GameObject _staticXZ;
        public GameObject StaticXZ => _staticXZ;

        //物理信息
        private readonly PhysicsInfo _info = PhysicsInfo.BuildInit();
#if UNITY_EDITOR
        public PhysicsInfo Info => _info;
#endif

        #region 外部接口

        

        #endregion
        
        #region 碰撞器生成

        //处理碰撞器
        private void HandleCol()
        {
            //如果没有物理配置，那么清理碰撞器
            if (!_info.HasPhyCol())
            {
                ClearAllCollider();
                return;
            }
            
            //遍历物理碰撞配置
            foreach (var item in _info.Physics)
            {
                //如果不在该帧生效，那么跳过
                if (!item.frameSequence.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }
                
                //如果有，那么执行配置
                if (_info.HasCol(item))
                {
                    ColModify(item);
                }
                //如果没有，那么添加新的
                else
                {
                    ColAdd(item);
                }
            }
            
            //不存在、不符合条件的，删除
            List<PhyCol> removeList = new List<PhyCol>();
            foreach (var item in _info.PhyColMap)
            {
                if (!_info.Physics.Contains(item.Key)
                    || !item.Key.frameSequence.Contains(_info.ActiveFrameIndex))
                {
                    removeList.Add(item.Key);
                }
            }
            
            ClearCollider(removeList);
        }

        //清理所有碰撞器
        private void ClearAllCollider()
        {
            //Component清理
            if (!CollectionUtil.IsEmpty(_info.PhyColMap))
            {
                foreach (var item in _info.PhyColMap)
                {
                    Destroy(item.Value);
                }
            }
            
            //存储清理
            _info.ClearAll();
        }

        //批量删除
        private void ClearCollider(List<PhyCol> phyCols)
        {
            if (CollectionUtil.IsEmpty(phyCols))
            {
                return;
            }

            foreach (var item in phyCols)
            {
                ClearCollider(item);
            }
        }

        //单个删除
        private void ClearCollider(PhyCol phyCol)
        {
            _info.PhyColMap.TryGetValue(phyCol, out var col);

            if (null != col)
            {
                Destroy(col);
            }
            
            //存储清理
            _info.Clear(phyCol);
        }

        //修正已有的碰撞盒
        private void ColModify(PhyCol phyCol)
        {
            _info.PhyColMap.TryGetValue(phyCol, out var col);
            
            //重新设置offset、size
            ResetCol(col, phyCol);
        }

        //添加新的碰撞盒
        private void ColAdd(PhyCol phyCol)
        {
            //按照类型添加
            BoxCollider2D col = null;
            switch (phyCol.box.colPos)
            {
                case ColPos.XZ:
                    col = _dynamicXZ.AddComponent<BoxCollider2D>();
                    break;
                case ColPos.Y:
                    col = _dynamicY.AddComponent<BoxCollider2D>();
                    break;
            }

            //重新设置offset、size
            ResetCol(col, phyCol);
            
            //写入存储
            _info.AddPhyCol(phyCol, col);
        }

        //重新设置碰撞器尺寸
        private void ResetCol(BoxCollider2D col, PhyCol phyCol)
        {
            if (null == col || null == phyCol || CollectionUtil.IsEmpty(phyCol.box.frameConfig))
            {
                return;
            }
            
            //设置为触发器
            col.isTrigger = true;
            
            //重设尺寸
            foreach (var item in phyCol.box.frameConfig)
            {
                if (item.frameSequence.Contains(_info.ActiveFrameIndex))
                {
                    //根据朝向重设offset信息
                    if (phyCol.box.orientationSensitive)
                    {
                        col.offset = new Vector2(item.offset.x * (Core.IsPositiveOrientation() ? 1 : -1), item.offset.y);
                    }
                    else
                    {
                        col.offset = item.offset;
                    }

                    col.size = item.size;
                }
            }
        }

        #endregion

        #region 碰撞处理

        

        #endregion
        
        #region 事件发生

        //发送碰撞器重设事件
        private void SendReColPost()
        {
            OnReColPost?.Invoke(_info);
        }
        
        #endregion
        
        #region AbstractObjUnit 接口

        public override void Init(ObjCore core)
        {
            base.Init(core);

            _dynamicXZ = gameObject.transform.Find(ActionConstant.Physics + ActionConstant.SplitSymbol +
                                         ActionConstant.DynamicCollider + ActionConstant.SplitSymbol +
                                         ActionConstant.DynamicColliderXZ).gameObject;
            _dynamicY = gameObject.transform.Find(ActionConstant.Physics + ActionConstant.SplitSymbol +
                                                   ActionConstant.DynamicCollider + ActionConstant.SplitSymbol +
                                                   ActionConstant.DynamicColliderY).gameObject;
            _staticXZ = gameObject.transform.Find(ActionConstant.Physics + ActionConstant.SplitSymbol +
                                                  ActionConstant.StaticColliderXZ).gameObject;
            
            _info.SetStatic(_staticXZ.GetComponent<BoxCollider2D>());
            
            HasInit = true;
        }

        public override void OnActionChangePost(ActionChangeEvent changeEvent)
        {
            //重置碰撞器，但是由于帧未处理，所以要等到帧变更事件，再进行处理
            _info.SetPhysics(changeEvent.PostActionSo);
            // Log.Info($"[PhysicsUnit]碰撞体清空准备。", LogModule.ObjCore);
        }

        public override void OnFrameChangePost(FrameChangeEvent changeEvent)
        {
            _info.SetFrameIndex(changeEvent.PostIndex);
            
            //开始处理
            HandleCol();

            //发送处理后事件
            SendReColPost();
        }

        public override void OnOrientationChangePost()
        {
            
        }

        #endregion

        #region 工具类

        //物理信息
        public class PhysicsInfo
        {
            //生效的帧序列
            public int ActiveFrameIndex { get; private set; }
            //行为物理
            public ActionPhysics ActionPhysics { get; private set; }
            //具体物理配置
            public List<PhyCol> Physics { get; private set; }
            
            //实时碰撞器配置
            public Dictionary<PhyCol, BoxCollider2D> PhyColMap { get; private set; }
            //实时碰撞器配置
            public Dictionary<BoxCollider2D, PhyCol> ColPhyMap { get; private set; }
            //
            // //实时碰撞器配置-XZ轴
            // public Dictionary<PhyCol, BoxCollider2D> XzMap { get; private set; }
            // //实时碰撞器配置-Y轴
            // public Dictionary<PhyCol, BoxCollider2D> YMap { get; private set; }
            //静态碰撞器-XZ轴
            public BoxCollider2D StaticCol { get; private set; }
            
            public static PhysicsInfo BuildInit()
            {
                PhysicsInfo info = new PhysicsInfo();
                info.ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                info.ActionPhysics = null;
                info.Physics = null;
                info.PhyColMap = new Dictionary<PhyCol, BoxCollider2D>();
                info.ColPhyMap = new Dictionary<BoxCollider2D, PhyCol>();
                    // info.XzMap = new Dictionary<PhyCol, BoxCollider2D>();
                // info.YMap = new Dictionary<PhyCol, BoxCollider2D>();
                info.StaticCol = null;
                return info;
            }

            //是否有物理碰撞器配置
            public bool HasPhyCol()
            {
                return ActionPhysics != null && !CollectionUtil.IsEmpty(Physics);
            }

            //是否有某个碰撞器
            public bool HasCol(PhyCol phyCol)
            {
                return PhyColMap.ContainsKey(phyCol);
            }

            //清理所有碰撞器
            public void ClearAll()
            {
                if (CollectionUtil.IsEmpty(ColPhyMap))
                {
                    return;
                }

                ActionPhysics = null;
                Physics = null;
                ColPhyMap.Clear();
                PhyColMap.Clear();
            }

            //单个删除
            public void Clear(PhyCol phyCol)
            {
                PhyColMap.TryGetValue(phyCol, out var col);
                if (null != col)
                {
                    PhyColMap.Remove(phyCol);
                    ColPhyMap.Remove(col);
                }
            }

            //设置静态碰撞器
            public void SetStatic(BoxCollider2D collider)
            {
                StaticCol = collider;
            }

            //设置物理信息
            public void SetPhysics(ActionSo action)
            {
                if (null == action.actionPhysics || CollectionUtil.IsEmpty(action.actionPhysics.infos))
                {
                    ActionPhysics = null;
                    Physics = null;   
                }
                else
                {
                    ActionPhysics = action.actionPhysics;
                    Physics = action.actionPhysics.infos;   
                }
            }

            //设置帧序列
            public void SetFrameIndex(int activeFrameIndex)
            {
                ActiveFrameIndex = activeFrameIndex;
            }

            //添加碰撞器
            public void AddPhyCol(PhyCol phyCol, BoxCollider2D collider)
            {
                if (!PhyColMap.ContainsKey(phyCol))
                {
                    PhyColMap.Add(phyCol, collider);
                }

                if (!ColPhyMap.ContainsKey(collider))
                {
                    ColPhyMap.Add(collider, phyCol);
                }
                
                //取消自身碰撞器之间的碰撞
                CancelColInteract(collider);
            }

            //根据碰撞器获取物理
            public PhyCol GetPhyByCol(BoxCollider2D col)
            {
                ColPhyMap.TryGetValue(col, out var phyCol);
                return phyCol;
            }

            //取消自身碰撞器之间的碰撞
            private void CancelColInteract(BoxCollider2D collider)
            {
                //取消与静态之前的碰撞
                if (null != StaticCol)
                {
                    Physics2D.IgnoreCollision(StaticCol, collider);
                }
                
                //取消动态之间的碰撞
                foreach (var item in ColPhyMap)
                {
                    if (item.Key == collider)
                    {
                        continue;
                    }
                    Physics2D.IgnoreCollision(item.Key, collider);
                }
            }
        }

        #endregion
    }
}