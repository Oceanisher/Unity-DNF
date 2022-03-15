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
        [Header("碰撞器组件Prefab")]
        [SerializeField]
        private GameObject colComponentPrefab;
        
        //重设碰撞器之后的回调
        public UnityAction<PhysicsInfo> OnReColPost;
        //碰撞进入回调
        public UnityAction<ColItemInfo> OnColEnter;
        //碰撞结束回调
        public UnityAction<ColItemInfo> OnColExit;

        #region 碰撞器变量

        //动态碰撞器父节点-XZ轴
        private GameObject _dynamicXZ;
        //动态碰撞器父节点-Y轴
        private GameObject _dynamicY;
        //静态碰撞器父节点-XZ轴
        private GameObject _staticXZ;

#if UNITY_EDITOR
        public GameObject DynamicXZ => _dynamicXZ;
        public GameObject DynamicY => _dynamicY;
        public GameObject StaticXZ => _staticXZ;
#endif

        #endregion

        //物理信息
        private readonly PhysicsInfo _info = PhysicsInfo.BuildInit();
        //碰撞信息
        private readonly PhysicsColInfo _colInfo = PhysicsColInfo.BuildInit();
        
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
            foreach (var item in _info.ActiveAction.actionPhysics.infos)
            {
                //如果不在该帧生效，那么跳过
                if (!item.frameSequence.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }

                foreach (var phyCol in item.colList)
                {
                    //如果有，那么执行配置
                    if (_info.HasCol(phyCol))
                    {
                        ColModify(phyCol);
                    }
                    //如果没有，那么添加新的
                    else
                    {
                        ColAdd(item, phyCol);
                    }
                }
            }
            
            //不存在、不符合条件的，删除
            List<PhyCol> removeList = new List<PhyCol>();
            foreach (var item in _info.PhyColMap)
            {
                bool isActive = false;
                foreach (var phy in _info.ActiveAction.actionPhysics.infos)
                {
                    if (phy.colList.Contains(item.Key)
                        && phy.frameSequence.Contains(_info.ActiveFrameIndex))
                    {
                        isActive = true;
                    }
                }

                if (!isActive)
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
                    Destroy(item.Value.gameObject);
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
                Destroy(col.gameObject);
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
        private void ColAdd(Phy phy, PhyCol phyCol)
        {
            //按照类型添加
            GameObject colGo = null;
            BoxCollider2D col = null;
            switch (phyCol.colPos)
            {
                case ColPos.XZ:
                    colGo = Instantiate(colComponentPrefab, _dynamicXZ.transform);
                    col = colGo.AddComponent<BoxCollider2D>();
                    break;
                case ColPos.Y:
                    colGo = Instantiate(colComponentPrefab, _dynamicY.transform);
                    col = colGo.AddComponent<BoxCollider2D>();
                    break;
            }

            if (null == colGo || null == col)
            {
                return;
            }
            
            //初始化组件
            colGo.GetComponent<PhysicsColUnit>().Init(this, phy, phyCol);

            //重新设置offset、size
            ResetCol(col, phyCol);
            
            //写入存储
            _info.AddPhyCol(phyCol, col);
        }

        //重新设置碰撞器尺寸
        private void ResetCol(BoxCollider2D col, PhyCol phyCol)
        {
            if (null == col || null == phyCol || CollectionUtil.IsEmpty(phyCol.frameConfig))
            {
                return;
            }
            
            //设置为触发器
            col.isTrigger = true;
            
            //重设尺寸
            foreach (var item in phyCol.frameConfig)
            {
                if (item.frameSequence.Contains(_info.ActiveFrameIndex))
                {
                    //根据朝向重设offset信息
                    if (phyCol.orientationSensitive)
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

        //碰撞发生处理
        public void OnCol(PhysicsColUnit self, PhysicsColUnit other, ColOccurType occurType)
        {
            lock (this)
            {
                //前置
                ColProcessPre(self, other, occurType);
                //处理
                ColProcess(out var removeList, out var processList);
                //后置
                ColProcessPost(removeList, processList);
            }
        }

        //碰撞前置处理
        private void ColProcessPre(PhysicsColUnit self, PhysicsColUnit other, ColOccurType occurType)
        {
            switch (occurType)
            {
                case ColOccurType.Enter:
                    _colInfo.ColEnter(self, other);
                    break;
                case ColOccurType.Stay:
                    _colInfo.ColStay(self, other);
                    break;
                case ColOccurType.Exit:
                    _colInfo.ColExit(self, other);
                    break;
            }
        }

        //碰撞前置处理
        private void ColProcess(out List<string> removeList, out List<string> processList)
        {
            removeList = new List<string>();
            processList = new List<string>();
            
            foreach (var item in _colInfo.colMap)
            {
                //TODO 如果对方行为已经失效/变更行为，那么不处理，未来不一定
                if (item.Value.OtherCore == null || !item.Value.OtherCore.GetActiveActionUuid().Equals(item.Key))
                {
                    removeList.Add(item.Key);
                    continue;
                }
                //XZ/Y状态相同、并且未处理，那么处理
                if (!item.Value.Processed)
                {
                    processList.Add(item.Key);
                }
            }
        }

        //碰撞后置处理
        private void ColProcessPost(List<string> removeList, List<string> processList)
        {
            //移除处理
            _colInfo.RemoveCol(removeList);
            //其他处理
            foreach (var item in processList)
            {
                _colInfo.colMap.TryGetValue(item, out var itemInfo);
                if (null == itemInfo)
                {
                    continue;
                }
                //碰撞进入处理
                if (itemInfo.IsEnterProcess())
                {
                    Log.Error($"[Physics]物体:{itemInfo.OtherCore.ObjConfigSo.objShowName} " +
                              $"使用:{itemInfo.OtherCore.GetActiveAction().actionShowName} " +
                              $"攻击了:{Core.ObjConfigSo.objShowName}, " +
                              $"造成:{itemInfo.OtherPhy.damage.damageOnce.damage}伤害", 
                        LogModule.ObjCore);
                    OnColEnter?.Invoke(itemInfo);
                }
                //碰撞离开处理
                if (itemInfo.IsExitProcess())
                {
                    Log.Error($"[Physics]物体:{itemInfo.OtherCore.ObjConfigSo.objShowName} " +
                              $"使用:{itemInfo.OtherCore.GetActiveAction().actionShowName} " +
                              $"攻击了:{Core.ObjConfigSo.objShowName}, " +
                              "完成", 
                        LogModule.ObjCore);
                    OnColExit?.Invoke(itemInfo);
                }
                
                //设置处理完成标识位
                itemInfo.SetProcessed();
            }
        }

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
                                         ActionConstant.Collider + ActionConstant.SplitSymbol +
                                         ActionConstant.DynamicColliderXZ).gameObject;
            _dynamicY = gameObject.transform.Find(ActionConstant.Physics + ActionConstant.SplitSymbol +
                                                   ActionConstant.Collider + ActionConstant.SplitSymbol +
                                                   ActionConstant.DynamicColliderY).gameObject;
            _staticXZ = gameObject.transform.Find(ActionConstant.Physics + ActionConstant.SplitSymbol +
                                                  ActionConstant.Collider + ActionConstant.SplitSymbol +
                                                  ActionConstant.StaticColliderXZ).gameObject;

            _info.SetStatic(_staticXZ.GetComponent<BoxCollider2D>());
            
            HasInit = true;
        }

        public override void OnActionChangePost(ActionChangeEvent changeEvent)
        {
            //重置碰撞器，但是由于帧未处理，所以要等到帧变更事件，再进行处理
            _info.SetPhysics(changeEvent.PostActionSo);
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
    }
    
    #region 工具类

        //物理信息
        public class PhysicsInfo
        {
            //生效的帧序列
            public int ActiveFrameIndex { get; private set; }
            //生效的行为
            public ActionSo ActiveAction { get; private set; }
            
            //实时碰撞器配置
            public Dictionary<PhyCol, BoxCollider2D> PhyColMap { get; private set; }
            
            //静态碰撞器-XZ轴
            public BoxCollider2D StaticCol { get; private set; }
            
            public static PhysicsInfo BuildInit()
            {
                PhysicsInfo info = new PhysicsInfo();
                info.ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                info.ActiveAction = null;
                info.PhyColMap = new Dictionary<PhyCol, BoxCollider2D>();
                info.StaticCol = null;
                return info;
            }

            //是否有物理碰撞器配置
            public bool HasPhyCol()
            {
                return ActiveAction != null
                       && null != ActiveAction.actionPhysics
                       && !CollectionUtil.IsEmpty(ActiveAction.actionPhysics.infos);
            }

            //是否有某个碰撞器
            public bool HasCol(PhyCol phyCol)
            {
                return PhyColMap.ContainsKey(phyCol);
            }

            //清理所有碰撞器
            public void ClearAll()
            {
                if (CollectionUtil.IsEmpty(PhyColMap))
                {
                    return;
                }

                ActiveAction = null;
                ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                PhyColMap.Clear();
            }

            //单个删除
            public void Clear(PhyCol phy)
            {
                PhyColMap.TryGetValue(phy, out var col);
                if (null != col)
                {
                    PhyColMap.Remove(phy);
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
                    ActiveAction = null;
                    ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                }
                else
                {
                    ActiveAction = action;
                    ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
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

                //取消自身碰撞器之间的碰撞
                CancelColInteract(collider);
            }

            //获取Phy
            public Phy GetPhy(PhyCol phyCol)
            {
                if (null == ActiveAction || null == ActiveAction.actionPhysics ||
                    CollectionUtil.IsEmpty(ActiveAction.actionPhysics.infos))
                {
                    return null;
                }

                foreach (var item in ActiveAction.actionPhysics.infos)
                {
                    if (CollectionUtil.IsEmpty(item.colList))
                    {
                        continue;
                    }

                    if (item.colList.Contains(phyCol))
                    {
                        return item;
                    }
                }

                return null;
            }

            //根据碰撞器获取物理
            public Phy GetPhy(BoxCollider2D col)
            {
                if (!PhyColMap.ContainsValue(col))
                {
                    return null;
                }

                foreach (var item in PhyColMap)
                {
                    if (item.Value == col)
                    {
                        return GetPhy(item.Key);
                    }
                }

                return null;
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
                foreach (var item in PhyColMap)
                {
                    if (item.Value == collider)
                    {
                        continue;
                    }
                    Physics2D.IgnoreCollision(item.Value, collider);
                }
            }
        }

        //物理碰撞信息
        public class PhysicsColInfo
        {
            //碰撞信息
            //key:行为uuid、value:碰撞信息
            public Dictionary<string, ColItemInfo> colMap { get; private set; }

            public static PhysicsColInfo BuildInit()
            {
                PhysicsColInfo info = new PhysicsColInfo();
                info.colMap = new Dictionary<string, ColItemInfo>();
                return info;
            }

            //碰撞进入
            public void ColEnter(PhysicsColUnit self, PhysicsColUnit other)
            {
                //获取对面行为的uuid
                string uuid = other.PhysicsUnit.Core.GetActiveActionUuid();

                colMap.TryGetValue(uuid, out var itemInfo);
                if (null == itemInfo)
                {
                    itemInfo = ColItemInfo.Build(self, other);
                    colMap.Add(uuid, itemInfo);
                }
                else
                {
                    itemInfo.ChangeColStatus(other.PhyCol.colPos, true);
                }
            }

            //碰撞保持
            public void ColStay(PhysicsColUnit self, PhysicsColUnit other)
            {
                //TODO 暂时不变，因为在Enter的时候已经加入了
            }

            //碰撞退出
            public void ColExit(PhysicsColUnit self, PhysicsColUnit other)
            {
                //获取对面行为的uuid
                string uuid = other.PhysicsUnit.Core.GetActiveActionUuid();

                colMap.TryGetValue(uuid, out var itemInfo);
                if (null != itemInfo)
                {
                    itemInfo.ChangeColStatus(other.PhyCol.colPos, false);
                }
            }
            
            //移除碰撞
            public void RemoveCol(List<string> removeList)
            {
                if (CollectionUtil.IsEmpty(removeList))
                {
                    return;
                }

                foreach (var item in removeList)
                {
                    colMap.Remove(item);
                }
            }
        }

        //碰撞单个信息
        public class ColItemInfo
        {
            //自身Core
            public ObjCore SelfCore;
            //对方Core
            public ObjCore OtherCore;
            //自身行为
            public ActionSo SelfAction;
            //对方行为
            public ActionSo OtherAction;
            //自身行为帧序列
            public int SelfFrameIndex;
            //对方行为帧序列
            public int OtherFrameIndex;
            //自身物理配置
            public Phy SelfPhy;
            //对方物理配置
            public Phy OtherPhy;
            //碰撞时间点
            public float Time;
            //XZ已碰撞
            public bool ColXz;
            //Y已碰撞
            public bool ColY;
            //是否已经处理过，仅当ColXz、ColY状态相同时才会变为true
            public bool Processed;

            public static ColItemInfo Build(PhysicsColUnit self, PhysicsColUnit other)
            {
                ColItemInfo info = new ColItemInfo();
                info.SelfCore = self.PhysicsUnit.Core;
                info.OtherCore = other.PhysicsUnit.Core;
                info.SelfAction = info.SelfCore.GetActiveAction();
                info.OtherAction = info.OtherCore.GetActiveAction();
                info.SelfFrameIndex = info.SelfCore.GetActiveActionFrameIndex();
                info.OtherFrameIndex = info.OtherCore.GetActiveActionFrameIndex();
                info.SelfPhy = self.Phy;
                info.OtherPhy = other.Phy;
                info.Time = TimeUtil.TimeMs();
                info.ColXz = other.PhyCol.colPos == ColPos.XZ;
                info.ColY = other.PhyCol.colPos == ColPos.Y;
                info.Processed = info.ColXz ^ info.ColY;
                return info;
            }

            //变更碰撞状态
            public void ChangeColStatus(ColPos colPos, bool isCol)
            {
                if (ColPos.XZ == colPos)
                {
                    ColXz = isCol;
                }
                else if (ColPos.Y == colPos)
                {
                    ColY = isCol;
                }
                Processed = ColXz ^ ColY;
            }

            //设置已处理标识位
            public void SetProcessed()
            {
                Processed = true;
            }

            //是否是完全碰撞的进入处理
            public bool IsEnterProcess()
            {
                return !Processed && ColXz == true && ColY == true;
            }

            //是否是完全离开的进入处理
            public bool IsExitProcess()
            {
                return !Processed && ColXz == false && ColY == false;
            }
        }
        
        //碰撞类型
        public enum ColOccurType
        {
            Enter,//进入
            Stay,//保持
            Exit,//退出
        }

        #endregion
}