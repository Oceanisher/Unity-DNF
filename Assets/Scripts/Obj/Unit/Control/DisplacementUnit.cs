using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obj.Config.Action.Structure;
using Obj.Event;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Obj.Unit.Control
{
    //位移单元
    //TODO 目前只处理内部位移，未来还要考虑外部条件、buff等对速度的影响。
    //TODO 即需要每帧处理位移、又需要接受帧变更信息
    public class DisplacementUnit : AbstractObjUnit
    {
        #region 事件委托
        
        //位移回调
        public UnityAction<DisplacementInfo> OnDisplacement;

        #endregion

        //位移信息
        private readonly DisplacementInfo _info = DisplacementInfo.BuildInit();

        #region 位移处理

        //位移处理
        private void HandleDis()
        {
            //位移前处理
            DisPre();
            //执行位移
            DisProcess();
            //位移后处理
            DisPost();
        }

        #region 位移处理-各个类型

        //匀速位移处理
        private void Handle_Move(Displacement info)
        {
            if (null == info.move)
            {
                return;
            }
            //已处理的不再处理
            _info.OnProcessMap.TryGetValue(info, out var hasProcessed);
            if (null == info.move || hasProcessed)
            {
                return;
            }

            SetPartVelocity_Move(info, info.move);
            
            //标识为已处理
            _info.MarkDisplacement(info);
        }

        //匀速按键按住位移处理
        private void Handle_MoveKeyHold(Displacement info)
        {
            if (null == info.moveKeyHold)
            {
                return;
            }
            
            //是否当前帧有按键
            bool hasKey = Core.HasKeyCurrentFrame(info.moveKeyHold.keys);
            if (hasKey)
            {
                SetPartVelocity_Move(info, info.moveKeyHold);
            }
            else
            {
                SetPartVelocity_Move(info, info.moveKeyHold, isCanceled:true);
            }
            
            //标识为已处理
            _info.MarkDisplacement(info);
        }

        //跳跃位移处理
        private void Handle_Jump(Displacement info)
        {
            if (null == info.jump)
            {
                return;
            }

            //如果已经开始处理，那么就按照加速度处理
            _info.OnProcessMap.TryGetValue(info, out var hasProcessed);
            _info.DisVelocityMap.TryGetValue(info, out var preVelocity);
            //如果未开始处理，那么给予一个初速度
            _info.AddOrUpdateVelocity(info, 
                new Vector3(
                    0f, 
                    hasProcessed ? 
                        preVelocity.y - info.jump.accelerate * TimeUtil.DeltaTime() : 
                        info.jump.startVelocity,
                    0f));

            //标识为已处理
            _info.MarkDisplacement(info);
        }
        
        //下落位移处理
        private void Handle_Drop(Displacement info)
        {
            if (null == info.drop)
            {
                return;
            }
            
            //按照加速度处理
            _info.OnProcessMap.TryGetValue(info, out var hasProcessed);
            _info.DisVelocityMap.TryGetValue(info, out var preVelocity);
            _info.AddOrUpdateVelocity(
                info, 
                new Vector3(
                    0f, 
                    hasProcessed ? 
                        preVelocity.y - info.drop.accelerate * TimeUtil.DeltaTime() : 
                        -info.drop.accelerate * TimeUtil.DeltaTime(), 
                    0f));
            
            //标识为已处理
            _info.MarkDisplacement(info);
        }

        //坠落位移处理
        private void Handle_Fall(Displacement info)
        {
            if (null == info.fall)
            {
                return;
            }
            
            //按照加速度处理
            _info.OnProcessMap.TryGetValue(info, out var hasProcessed);
            _info.DisVelocityMap.TryGetValue(info, out var preVelocity);
            _info.AddOrUpdateVelocity(
                info, 
                new Vector3(
                    0f, 
                    hasProcessed ? 
                        preVelocity.y - info.drop.accelerate * TimeUtil.DeltaTime() : 
                        -info.drop.accelerate * TimeUtil.DeltaTime(), 
                    0f));            // _info.AddSpeedY(-info.fall.accelerate * TimeUtil.DeltaTime());
            
            //标识为已处理
            _info.MarkDisplacement(info);
        }

        #endregion
        
        #region 位移处理-工具方法
        
        //位移前处理
        private void DisPre()
        {
            //如果顿帧进行中，那么不进行任何移动计算
            if (Core.GetActionFrameFreeze())
            {
                return;
            }
            
            if (CollectionUtil.IsEmpty(_info.ActiveDisplacementInfos))
            {
                return;
            }

            foreach (var item in _info.ActiveDisplacementInfos)
            {
                switch (item.type)
                {
                    //匀速处理
                    case DisplacementType.Move:
                        Handle_Move(item);
                        break;
                    //匀速按键处理
                    case DisplacementType.MoveKeyHold:
                        Handle_MoveKeyHold(item);
                        break;
                    //跳跃处理
                    case DisplacementType.Jump:
                        Handle_Jump(item);
                        break;
                    //下落处理
                    case DisplacementType.Drop:
                        Handle_Drop(item);
                        break;
                    //坠落处理
                    case DisplacementType.Fall:
                        Handle_Fall(item);
                        break;
                    //滞空处理
                    case DisplacementType.SkyStay:
                        break;
                }
            }
        }
        
        //位移处理
        private void DisProcess()
        {
            //如果顿帧进行中，那么不进行任何移动计算，取消各类移动
            if (Core.GetActionFrameFreeze())
            {
                SetFinalVelocityOnFreeze();
                return;
            }
            
            //先计算总体速度
            _info.CalculateFinalVelocity();
            
            //XYZ各轴移动处理，转换为游戏对象的移动
            SetFinalVelocity();
        }

        //位移后处理
        private void DisPost()
        {
            //如果顿帧进行中，那么不进行任何移动计算
            if (Core.GetActionFrameFreeze())
            {
                return;
            }
            
            //状态变更
            _info.StateChange(Core);
            
            //回调
            SendDisplacementEvent();
        }

        //转换速度到游戏物体真实速度，地面速度，Y轴移动另算
        private Vector3 ConvertVelocityGround(Vector3 vec)
        {
            return new Vector3(vec.x, vec.z, 0);
        }

        //设置最终速度
        private void SetFinalVelocity()
        {
            //XYZ各轴移动处理，转换为游戏对象的移动
            SetVelocityXZ();
            SetVelocityY();
        }

        //设置顿帧期间的移动速度
        private void SetFinalVelocityOnFreeze()
        {
            Core.GetRb().velocity = Vector2.zero;
        }

        //设置地面速度，Y轴另算
        private void SetVelocityXZ()
        {
            Core.GetRb().velocity = ConvertVelocityGround(_info.VelocityInner);
        }

        //设置真实Y轴速度，也就是localY的速度
        private void SetVelocityY()
        {
            //处理下落到达地面
            //如果位置已经到达地面、并且目前是在下降，那么速度置为0
            if (Core.GetGraphicsGo().transform.localPosition.y <= 0
                && _info.VelocityInner.y <= float.Epsilon)
            {
                Core.GetGraphicsGo().transform.localPosition = Vector3.zero;
                // Core.GetPhysicsGo().transform.localPosition = Vector3.zero;
                _info.ResetInnerVelocityY();
            }
            //处理其他空中情况
            else
            {
                float yMove = _info.VelocityInner.y * TimeUtil.DeltaTime();
                Vector3 temp = Core.GetGraphicsGo().transform.localPosition;
                temp.y += yMove;
                Core.GetGraphicsGo().transform.localPosition = temp;
            }
        }

        //内部类型、匀速类型增加速度
        //isAdd:是添加还是减少
        //isCanceled:是取消速度还是改变速度
        private void SetPartVelocity_Move(Displacement dis, DisplacementMove move, bool isAdd = true, bool isCanceled = false)
        {
            //如果是取消速度
            if (isCanceled)
            {
                _info.AddOrUpdateVelocity(dis, Vector3.zero);
                return;
            }
            
            //非取消速度
            Vector3 startVelocity = Vector3.zero;
            switch (move.velocityType)
            {
                case DisplacementVelocityType.CharacterWalk:
                    startVelocity = Core.Properties_WalkSpeed();
                    break;
                case DisplacementVelocityType.CharacterRun:
                    startVelocity = Core.Properties_RunSpeed();
                    break;
                case DisplacementVelocityType.Custom:
                    startVelocity = move.customVelocity;
                    break;
            }

            Vector3 temp = Vector3.zero;
            switch (move.orientationType)
            {
                case DisplacementOrientationType.Forward:
                    temp += new Vector3((Core.IsPositiveOrientation() ? 1 : -1) * startVelocity.x, 0, 0);
                    break;
                case DisplacementOrientationType.Backward:
                    temp += new Vector3((Core.IsPositiveOrientation() ? -1 : 1) * startVelocity.x, 0, 0);
                    break;
                case DisplacementOrientationType.XForward:
                    temp += new Vector3(startVelocity.x, 0, 0);
                    break;
                case DisplacementOrientationType.XBackward:
                    temp += new Vector3(-startVelocity.x, 0, 0);
                    break;
                case DisplacementOrientationType.ZForward:
                    temp += new Vector3(0, 0, startVelocity.z);
                    break;
                case DisplacementOrientationType.ZBackward:
                    temp += new Vector3(0, 0, -startVelocity.z);
                    break;
                default:
                    break;
            }

            _info.AddOrUpdateVelocity(dis, isAdd ? temp : -temp);
        }

        #endregion

        #endregion

        #region 事件发出

        //发送位移变更事件
        private void SendDisplacementEvent()
        {
            OnDisplacement?.Invoke(_info);
        }

        #endregion

        #region AbstractObjUnit 接口

        public override void Init(ObjCore core)
        {
            base.Init(core);
            HasInit = true;
        }

        public override void InnerLateUpdate()
        {
            //位移处理
            HandleDis();
        }

        public override void OnActionChangePost(ActionChangeEvent changeEvent)
        {
            _info.RemoveAllDisplacement();
            //重置当前其他数据
            _info.ResetWhenActionChange(changeEvent);
        }

        public override void OnFrameChangePost(FrameChangeEvent changeEvent)
        {
            //只关注帧变更，行为变更暂不处理
            _info.ChangeFrame(changeEvent.PostIndex);
            ActionDisplacement displacement = Core.GetActiveDisplacement();
            if (null != displacement && !CollectionUtil.IsEmpty(displacement.infos))
            {
                //添加新的
                foreach (var item in displacement.infos)
                {
                    if (item.frameSequence.Contains(_info.ActiveFrameIndex)
                        && !_info.ActiveDisplacementInfos.Contains(item))
                    {
                        _info.AddNewDisplacement(item);
                    }
                }
                //移除旧的、不包含该帧处理的位移
                for (int i = _info.ActiveDisplacementInfos.Count() - 1; i >= 0; i--)
                {
                    Displacement dis = _info.ActiveDisplacementInfos[i];
                    if (!dis.frameSequence.Contains(_info.ActiveFrameIndex)
                        || !displacement.infos.Contains(dis))
                    {
                        _info.RemoveDisplacement(_info.ActiveDisplacementInfos[i]);
                    }
                }
            }
            else
            {
                _info.RemoveAllDisplacement();
            }
        }

        #endregion
    }
    
    #region 工具类

        //位移信息
        public class DisplacementInfo
        {
            //当前处理的帧位移
            public List<Displacement> ActiveDisplacementInfos { get; private set; }
            //当前处理中的位移
            public Dictionary<Displacement, bool> OnProcessMap { get; private set; }
            //处理中的位移带来的速度
            public Dictionary<Displacement, Vector3> DisVelocityMap { get; private set; }
            //当前处理帧位移的帧
            public int ActiveFrameIndex { get; private set; }

            //当前的三轴速度-内部
            public Vector3 VelocityInner { get; private set; }
            
            //是否在地面
            public bool OnGround { get; private set; }
            
            //是否在上升中
            public bool OnRising { get; private set; }

            //当前位置，XZ为世界坐标、Y为本地坐标
            public Vector3 Position { get; private set; }
            
            //是否到达过最高点
            public bool ArriveHighest { get; private set; }
            
            //最高点高度
            public float Highest { get; private set; }

            public static DisplacementInfo BuildInit()
            {
                DisplacementInfo info = new DisplacementInfo();
                info.ActiveDisplacementInfos = new List<Displacement>();
                info.OnProcessMap = new Dictionary<Displacement, bool>();
                info.DisVelocityMap = new Dictionary<Displacement, Vector3>();
                info.ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                info.VelocityInner = Vector3.zero;
                info.OnGround = true;
                info.OnRising = false;
                info.Position = Vector3.zero;
                info.ArriveHighest = false;
                info.Highest = 0f;
                return info;
            }

            //切换帧
            public void ChangeFrame(int index)
            {
                ActiveFrameIndex = index;
            }

            //重置角色其他数据
            public void ResetWhenActionChange(ActionChangeEvent changeEvent)
            {
                //TODO 如果是变成跳起，那么重置到达顶点的动作
                if (changeEvent.PostActionSo.type == ActionType.CharacterJump)
                {
                    ArriveHighest = false;
                    OnRising = true;
                    Highest = 0f;
                }
            }

            //重置Y轴速度为0
            public void ResetInnerVelocityY()
            {
                VelocityInner = new Vector3(VelocityInner.x, 0f, VelocityInner.z);
            }

            //添加或更新速度
            public void AddOrUpdateVelocity(Displacement dis, Vector3 velocity)
            {
                if (DisVelocityMap.ContainsKey(dis))
                {
                    DisVelocityMap.Remove(dis);
                }
                DisVelocityMap.Add(dis, velocity);
            }

            //计算最终速度，落到VelocityInner上
            //TODO 目前只计算内部速度
            public void CalculateFinalVelocity()
            {
                //先重置速度
                VelocityInner = Vector3.zero;
                
                if (CollectionUtil.IsEmpty(DisVelocityMap))
                {
                    return;
                }

                foreach (var item in DisVelocityMap)
                {
                    //如果该行为不包含当前帧，那么跳过
                    //TODO 后续有些行为结束还能导致位移的技能，需要其他处理
                    if (!item.Key.frameSequence.Contains(ActiveFrameIndex)
                        || !ActiveDisplacementInfos.Contains(item.Key))
                    {
                        continue;
                    }
                    VelocityInner += item.Value;
                }
            }
            
            //判断最终结果是否需要移动
            //TODO 目前只计算内部速度
            public bool ShouldMove()
            {
                return Mathf.Abs(VelocityInner.x) > float.Epsilon
                       || Mathf.Abs(VelocityInner.y) > float.Epsilon
                       || Mathf.Abs(VelocityInner.z) > float.Epsilon;
            }

            //状态变更
            public void StateChange(ObjCore core)
            {
                //状态变更
                OnGround = core.GetGraphicsGo().transform.localPosition.y <= float.Epsilon;
                bool preRising = OnRising;
                OnRising = VelocityInner.y > float.Epsilon;
                //如果之前是上升、现在不是了，那么说明到达了最高点
                if (preRising && !OnRising)
                {
                    ArriveHighest = true;
                    Highest = core.GetGraphicsGo().transform.localPosition.y;
                }
                //如果之前不是上升，现在是上升，那么说明是上升开始
                else if (!preRising && OnRising)
                {
                    ArriveHighest = false;
                }
            }
            
            //添加新的位移处理
            public void AddNewDisplacement(Displacement displacement)
            {
                ActiveDisplacementInfos.Add(displacement);
                OnProcessMap.Add(displacement, false);
            }
        
            //移除单个位移处理
            public void RemoveDisplacement(Displacement displacement)
            {
                ActiveDisplacementInfos.Remove(displacement);
                OnProcessMap.Remove(displacement);
                //历史位移数据清除
                //TODO 后续有些行为结束还能导致位移的技能，需要其他处理
                if (DisVelocityMap.ContainsKey(displacement))
                {
                    DisVelocityMap.Remove(displacement);
                }
            }

            //移除所有位移处理
            public void RemoveAllDisplacement()
            {
                ActiveDisplacementInfos.Clear();
                OnProcessMap.Clear();
                //TODO 后续有些行为结束还能导致位移的技能，需要其他处理
                DisVelocityMap.Clear();
            }

            //标记位移为已处理
            public void MarkDisplacement(Displacement displacement)
            {
                OnProcessMap.Remove(displacement);
                OnProcessMap.Add(displacement, true);
            }
        
            //标记位移为未处理
            public void MarkDisplacementClear(Displacement displacement)
            {
                OnProcessMap.Remove(displacement);
                OnProcessMap.Add(displacement, false);
            }

            //获取当前下落的百分比
            public int GetFallPercent()
            {
                //如果最大高度为0，那么返回一个错误信息
                if (Highest <= float.Epsilon)
                {
                    return ActionConstant.InvalidFallPercent;
                }

                return (int)((Highest - Position.y) / Highest * 100);
            }
        }

        #endregion
}