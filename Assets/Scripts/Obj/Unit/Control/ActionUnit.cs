using System;
using System.Collections;
using System.Collections.Generic;
using Obj.Config.Action;
using Obj.Config.Action.Structure;
using Obj.Config.Property;
using Obj.Event;
using Sys.Config;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Obj.Unit.Control
{
    //行为单元
    public class ActionUnit : AbstractObjUnit
    {
        #region 事件委托

        //物体终结
        public UnityAction OnObjDeath;

        //行为变更前
        public UnityAction<ActionChangeEvent> OnActionPre;

        //行为变更后
        public UnityAction<ActionChangeEvent> OnActionPost;

        //帧变更前
        public UnityAction<FrameChangeEvent> OnFramePre;

        //帧变更后
        public UnityAction<FrameChangeEvent> OnFramePost;

        #endregion

        #region 对外变量

        //当前生效行为
        public ActionSo ActiveAction => _info.ActiveAction;

        //当前帧序列
        public int ActiveFrameIndex => _info.ActiveFrameIndex;

        //当前帧信息
        public AniFrameInfo ActiveFrameInfo => _info.ActiveFrame;

        #endregion

        //内部行为相关信息
        private readonly ActionInfo _info = ActionInfo.BuildInit();
        public ActionInfo Info => _info;

        #region 外部开放接口

        //角色类状态变更
        public void CharacterStateChange(ObjPosState preState, ObjPosState state)
        {
            //TODO 从地面到空中、到达最高点、从空中到地面
            //例如技能结束，那么状态变更是OjbCore决定的
        }

        //角色类位移变更
        public void CharacterDisplacementChange(DisplacementInfo info)
        {
            //检查是否需要从跳->下落
            if (_info.ActiveAction.type == ActionType.CharacterJump
                && CheckActionJumpToFall(info))
            {
                Log.DebugInfo("[ActionUnit]行为变更-从跳切换到下落。", LogModule.ObjCore);
                ChangeAction(Core.GetAction(ActionType.CharacterDrop));
            }

            //检查是否需要从下落->默认
            if (_info.ActiveAction.type == ActionType.CharacterDrop
                && CheckActionDropToGround(info))
            {
                Log.DebugInfo("[ActionUnit]行为变更-从下落切换到默认。", LogModule.ObjCore);
                ChangeAction(GetDefaultAction());
            }
        }
        
        //变更行为-AI
        public void ActionChange_AI(ActionSo actionSo, int frameIndex)
        {
            _info.ChangeToAi(actionSo, frameIndex);
        }

        //变更行为-手动
        public void ActionChange_Manual()
        {
            _info.ChangeToManual();
        }

        //被动碰撞处理
        public void ColEnterPassive(ColItemInfo info)
        {
        }

        //主动碰撞处理
        public void ColEnterActive(ColItemInfo info)
        {
            //如果自身行为是可以顿帧的，并且该帧尚未顿帧过，那么可以进行顿帧
            if (info.OtherAction.type != _info.ActiveAction.type
                || info.OtherFrameIndex != _info.ActiveFrameIndex)
            {
                return;
            }

            //该帧已经顿帧过、或正在顿帧中，跳过
            if (_info.HasFreezeOver || _info.OnFreezeProcess)
            {
                return;
            }
            
            _info.FreezeStart();
        }

        #endregion

        #region 行为状态机

        #region 行为状态机-核心

        //运转状态机
        private void RunStateMachine()
        {
            if (!HasInit)
            {
                return;
            }

            //运转状态机前处理
            PreRunStateMachine();

            ObjType type = Core.Properties.InherentProperties.ObjType;
            switch (type)
            {
                case ObjType.Character:
                    StateMachineCharacter();
                    break;
                case ObjType.Active:
                    StateMachineActive();
                    break;
                case ObjType.Effect:
                    StateMachineEffect();
                    break;
            }

            //运转状态机后处理
            PostRunStateMachine();
        }

        #region 行为状态机-角色类

        //行为状态机-角色类
        private void StateMachineCharacter()
        {
            //如果当前行为为空，那么回归默认
            if (null == _info.ActiveAction)
            {
                Log.DebugInfo("[ActionUnit]行为变更-初始化。", LogModule.ObjCore);
                ChangeAction(GetDefaultAction());
                return;
            }

            if (_info.IsAI)
            {
                HandleAction_Ai();
            }
            else
            {
                HandleAction_Manual();   
            }
        }

        //行为变更检查-有无按键
        private bool ActionChangeCheck_Key(out ActionSo changeAction)
        {
            if (CollectionUtil.IsEmpty(Core.FrameKeys))
            {
                changeAction = null;
                return false;
            }

            ActionType actionType = Core.ObjConfigSo.GetActionByKeys(Core.FrameKeys);
            ActionSo actionSo = Core.GetAction(actionType);

            //如果行为为空，那么不改变
            if (null == actionSo)
            {
                changeAction = null;
                return false;
            }
            
            //行为联动检查，如果联动可以生效，那么设置标识位
            if (_info.CanChangeLinkage(actionSo))
            {
                _info.ActiveLinkage();
                changeAction = null;
                return false;
            }
            
            //如果不满足联动、而且是当前行为，那么跳过
            if (_info.ActiveAction.type == actionSo.type)
            {
                changeAction = null;
                return false;
            }

            changeAction = actionSo;
            return null != actionSo;
        }

        //处理当前行为
        //先看行为是否需要中止；如果未中止，那么看能否切换到下一帧
        private void HandleAction_Manual()
        {
            //如果是从手动变更为AI，那么初始化
            if (_info.AiChange)
            {
                ChangeAction(GetDefaultAction());
                _info.AiTagCancel();
                return;
            }
            
            //按键变更检查
            if (ActionChangeCheck_Key(out var changeAction))
            {
                //行为变更冲突检查
                if (ActionConflictCheck(changeAction, _info.ActiveAction))
                {
                    Log.DebugInfo($"[ActionUnit]行为变更-按键。action:{changeAction.actionName}", LogModule.ObjCore);
                    ChangeAction(changeAction);
                    return;
                }
            }
            
            //行为中止检查
            if (CheckActionStop())
            {
                Log.DebugInfo($"[ActionUnit]行为中止-从{_info.ActiveAction.name}切换到默认。", LogModule.ObjCore);
                ChangeAction(GetDefaultAction());
                return;
            }
            
            //下一帧切换检查
            if (!CheckFrameNext())
            {
                return;
            }

            //能够切换到下一帧，那么要判断是否有下一帧、如果是循环的那么要切换到第一帧
            int nextFrameIndex = GetNextFrameIndex();
            //如果无法切换（错误、或者已经到达最后一帧），那么该行为结束
            if (nextFrameIndex == ActionConstant.InvalidFrameIndex)
            {
                Log.DebugInfo($"[ActionUnit]行为结束/错误-从{_info.ActiveAction.name}切换到默认。", LogModule.ObjCore);
                //尝试切换到联动行为
                ChangeAction(_info.Linkage.CanChangeLinkage ? _info.Linkage.PostAction : GetDefaultAction());
            }
            //正常切换
            else
            {
                if (0 == nextFrameIndex)
                {
                    ResetFrame();
                }
                else
                {
                    NextFrame(nextFrameIndex);
                }
            }
        }
        
        //处理当前行为
        //如果Ai的帧无效，那么默认是持续该帧
        private void HandleAction_Ai()
        {
            //如果是从手动变更为AI，那么初始化
            if (_info.AiChange)
            {
                ChangeAction(_info.AiAction);
                _info.AiTagCancel();
                return;
            }
            
            //如果是无效帧，那么默认循环该行为
            if (_info.AiFrameIndex == ActionConstant.InvalidFrameIndex)
            {
                //下一帧切换检查
                if (!CheckFrameNext())
                {
                    return;
                }

                //能够切换到下一帧，那么要判断是否有下一帧、如果是循环的那么要切换到第一帧
                int nextFrameIndex = GetNextFrameIndex();
                //如果无法切换（错误、或者已经到达最后一帧），那么该行为重置到第一帧
                if (nextFrameIndex == ActionConstant.InvalidFrameIndex)
                {
                    ResetFrame();
                }
                //正常切换
                else
                {
                    if (0 == nextFrameIndex)
                    {
                        ResetFrame();
                    }
                    else
                    {
                        NextFrame(nextFrameIndex);
                    }
                }
            }
            //如果是有效帧，那么持续该帧不变
            else
            {
                if (_info.ActiveFrameIndex != _info.AiFrameIndex)
                {
                    NextFrame(_info.AiFrameIndex);
                }
            }
        }

        #endregion

        #region 行为状态机-活跃物体类

        //行为状态机-活跃物体类
        private void StateMachineActive()
        {

        }

        #endregion

        #region 行为状态机-特效类

        //行为状态机-特效类
        private void StateMachineEffect()
        {

        }

        #endregion

        #endregion

        #region 行为状态机-工具方法

        //状态机帧前处理
        private void PreRunStateMachine()
        {
            _info.UpdatePre();
        }

        //状态机帧后处理
        private void PostRunStateMachine()
        {
            //TODO 变更角色状态
            //朝向变更
            ChangeOrientation();
        }

        #region 行为状态机-工具方法-行为变更相关

        #region 行为中止检查

        //行为中止检查：该行为是否需要中止
        private bool CheckActionStop()
        {
            if (_info.ActiveAction.actionOther == null
                || CollectionUtil.IsEmpty(_info.ActiveAction.actionOther.actionStopCondition))
            {
                return false;
            }

            //任意条件满足后，中止行为
            foreach (var item in _info.ActiveAction.actionOther.actionStopCondition)
            {
                if (!item.frameIndexes.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }

                bool canNext = false;
                switch (item.type)
                {
                    case ActionSwitchType.KeyInvalid:
                        canNext = ActionSwitchKeyInvalid(item);
                        break;
                }

                if (canNext)
                {
                    return true;
                }
            }

            return false;
        }

        //跳起行为中止检查：该行为是否需要中止
        private bool CheckActionJumpToFall(DisplacementInfo info)
        {
            if (_info.ActiveAction.actionOther == null
                || CollectionUtil.IsEmpty(_info.ActiveAction.actionOther.actionStopCondition))
            {
                return false;
            }

            //任意条件满足后，中止行为
            foreach (var item in _info.ActiveAction.actionOther.actionStopCondition)
            {
                if (!item.frameIndexes.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }

                if (ActionSwitchType.JumpToTop == item.type)
                {
                    return ActionSwitchJumpToTop(info);
                }
            }

            return false;
        }

        //下落行为中止检查：该行为是否需要中止
        private bool CheckActionDropToGround(DisplacementInfo info)
        {
            if (_info.ActiveAction.actionOther == null
                || CollectionUtil.IsEmpty(_info.ActiveAction.actionOther.actionStopCondition))
            {
                return false;
            }

            //任意条件满足后，中止行为
            foreach (var item in _info.ActiveAction.actionOther.actionStopCondition)
            {
                if (!item.frameIndexes.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }

                if (ActionSwitchType.FallToGround == item.type)
                {
                    return ActionSwitchFallToGround(info);
                }
            }

            return false;
        }

        //行为中止校验-按键无效
        private bool ActionSwitchKeyInvalid(ActionSwitchCondition condition)
        {
            return !Core.HasKeyCurrentFrame(condition.keyInvalid.keys);
        }

        //行为中止校验-跳起时到达顶点
        private bool ActionSwitchJumpToTop(DisplacementInfo info)
        {
            return info.ArriveHighest;
        }

        //行为中止校验-下落时到达地面
        private bool ActionSwitchFallToGround(DisplacementInfo info)
        {
            return info.OnGround;
        }

        #endregion

        //行为变更
        private void ChangeAction(ActionSo actionSo)
        {
            //变更前
            SendActionChangePre(_info.PreAction, actionSo);

            //执行变更
            ChangeActionInner(actionSo);

            //变更后
            SendActionChangePost(_info.PreAction, _info.ActiveAction);

            //初始化帧
            InitFrame();
        }

        //行为变更，实际执行
        private void ChangeActionInner(ActionSo actionSo)
        {
            _info.ChangeAction(actionSo);
            //如果有新的联动，那么变更联动；如果没有联动，那么无需才做，等待上一个联动结束即可
            if (null != actionSo.actionOther.linkage.linkageAction)
            {
                _info.SetLinkage(actionSo);
            }
        }

        //获取默认行为
        private ActionSo GetDefaultAction()
        {
            //非角色类，直接返回城镇默认
            if (ObjType.Character != Core.InherentProperties.ObjType)
            {
                return Core.GetAction(ActionType.CharacterDefaultTown);;
            }

            //角色类要根据场景、角色类型判断
            switch (Core.GetSceneType())
            {
                //TODO 默认返回城镇默认行为
                case SceneType.None:
                    return Core.GetAction(ActionType.CharacterDefaultTown);
                case SceneType.Town:
                    return Core.GetAction(ActionType.CharacterDefaultTown);
                case SceneType.Fight:
                    switch (Core.InherentProperties.CharacterInherentProperties.FlyType)
                    {
                        //默认返回战斗陆地行为
                        case FlyType.None:
                            return Core.GetAction(ActionType.CharacterDefaultFight);
                        case FlyType.Ground:
                            return Core.GetAction(ActionType.CharacterDefaultTown);
                        case FlyType.Fly:
                            return Core.GetAction(ActionType.CharacterDefaultFight);
                        //混合类默认返回飞行行为
                        case FlyType.GroundFly:
                            return Core.GetAction(ActionType.CharacterDefaultFight);
                    }

                    break;
            }

            return Core.GetAction(ActionType.CharacterDefaultTown);;
        }

        //行为冲突检查
        private bool ActionConflictCheck(ActionSo next, ActionSo current)
        {
            //走、跑未结束时，不能互相切换
            if ((next.type == ActionType.CharacterWalk
                 && current.type == ActionType.CharacterRun)
                || (next.type == ActionType.CharacterRun
                    && current.type == ActionType.CharacterWalk))
            {
                return false;
            }

            //跳、坠落/降落的过程中，不能互相切换
            if ((current.type == ActionType.CharacterDrop
                 || current.type == ActionType.CharacterFall)
                && next.type == ActionType.CharacterJump)
            {
                return false;
            }

            //跳、坠落、降落的过程中，不能切换到走、跑
            if ((current.type == ActionType.CharacterJump
                 || current.type == ActionType.CharacterDrop
                 || current.type == ActionType.CharacterFall)
                && (next.type == ActionType.CharacterWalk
                    || next.type == ActionType.CharacterRun))
            {
                return false;
            }

            //普攻过程中，不能切换到其他
            //TODO 后续可以有强制技能
            if (current.type == ActionType.CharacterHit
                && next.type != ActionType.CharacterHit)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 行为状态机-工具方法-帧变更相关

        #region 下一帧切换检查

        //帧切换检查：该帧是否需要切换
        private bool CheckFrameNext()
        {
            if (CollectionUtil.IsEmpty(_info.ActiveAction.actionFrame.frameSwitchConditions))
            {
                return false;
            }

            //任意条件满足后，可切换到下一帧
            foreach (var item in _info.ActiveAction.actionFrame.frameSwitchConditions)
            {
                if (!item.frameIndexes.Contains(_info.ActiveFrameIndex))
                {
                    continue;
                }

                bool canNext = false;
                bool freezeOver = false;
                //时间类型的需要判断是否需要顿帧
                switch (item.type)
                {
                    case FrameSwitchType.Time:
                        canNext = FrameSwitchTime(item) && FreezeTimeCheck(out freezeOver);
                        break;
                    case FrameSwitchType.TimeOrKey:
                        canNext = FrameSwitchTimeOrKey(item) && FreezeTimeCheck(out freezeOver);
                        break;
                    case FrameSwitchType.TimeOrKeyInvalid:
                        canNext = FrameSwitchTimeOrKeyInvalid(item) && FreezeTimeCheck(out freezeOver);
                        break;
                    case FrameSwitchType.Key:
                        canNext = FrameSwitchKey(item);
                        break;
                    case FrameSwitchType.KeyInvalid:
                        canNext = FrameSwitchKeyInvalid(item);
                        break;
                }
                
                //判断是否顿帧结束
                if (freezeOver)
                {
                    _info.FreezeOver();
                }
                
                if (canNext)
                {
                    return true;
                }
            }

            return false;
        }

        //顿帧检测
        //freezeOver:是否结束顿帧
        private bool FreezeTimeCheck(out bool freezeOver)
        {
            if (!_info.OnFreezeProcess)
            {
                freezeOver = false;
                return true;
            }

            //顿帧的持续时间大于配置时间，那么该帧的顿帧时间结束
            freezeOver = _info.FreezeLastTime > _info.ActiveAction.actionFrame.frameFreezeInfo.time;
            return freezeOver;
        }

        //帧切换校验-时间
        private bool FrameSwitchTime(FrameSwitchCondition condition)
        {
            return _info.ActiveFrameTime >= condition.time.time;
        }

        //帧切换校验-时间&按键
        private bool FrameSwitchTimeOrKey(FrameSwitchCondition condition)
        {
            return _info.ActiveFrameTime >= condition.timeOrKey.time
                   || Core.HasKeyCurrentFrame(condition.timeOrKey.keys);
        }

        //帧切换校验-时间&按键无效
        private bool FrameSwitchTimeOrKeyInvalid(FrameSwitchCondition condition)
        {
            return _info.ActiveFrameTime >= condition.timeOrKeyInvalid.time
                   || !Core.HasKeyCurrentFrame(condition.timeOrKeyInvalid.keys);
        }

        //帧切换校验-按键
        private bool FrameSwitchKey(FrameSwitchCondition condition)
        {
            return Core.HasKeyCurrentFrame(condition.key.keys);
        }

        //帧切换校验-按键无效
        private bool FrameSwitchKeyInvalid(FrameSwitchCondition condition)
        {
            return !Core.HasKeyCurrentFrame(condition.keyInvalid.keys);
        }

        //帧切换校验-下落高度百分比
        private bool FrameSwitchFallPercent(FrameSwitchCondition condition, DisplacementInfo info)
        {
            int percent = info.GetFallPercent();
            if (percent == ActionConstant.InvalidFallPercent)
            {
                return false;
            }

            return percent >= condition.fallPercent.percent;
        }

        #endregion

        //初始化帧
        private void InitFrame()
        {
            //变更前
            SendFrameChangePre(_info.ActiveFrameIndex, ActionConstant.InvalidFrameIndex);
            //执行变更
            InitFrameInner();
            //变更后
            SendFrameChangePost(ActionConstant.InvalidFrameIndex, _info.ActiveFrameIndex);
        }

        //初始化帧，实际执行
        private void InitFrameInner()
        {
            _info.InitFrame();
        }

        //下一帧
        private void NextFrame(int index)
        {
            //变更前
            SendFrameChangePre(_info.ActiveFrameIndex, index);
            //执行变更
            NextFrameInner(index);
            //变更后
            SendFrameChangePost(_info.PreFrameIndex, _info.ActiveFrameIndex);
        }

        //下一帧，实际执行
        private void NextFrameInner(int index)
        {
            _info.NextFrame(index);
        }

        //重置到第一帧
        private void ResetFrame()
        {
            //变更前
            SendFrameChangePre(_info.ActiveFrameIndex, 0);
            //执行变更
            ResetFrameInner();
            //变更后
            SendFrameChangePost(_info.PreFrameIndex, _info.ActiveFrameIndex);
        }

        //重置到第一帧
        private void ResetFrameInner()
        {
            _info.ResetFrame();
        }

        //获取下一帧的序列
        //如果是最后一帧、并且是循环，那么回到第一帧
        private int GetNextFrameIndex()
        {
            //未到结尾
            if (_info.ActiveFrameIndex < _info.ActiveAction.actionFrame.aniFrameInfos.Count - 1)
            {
                return _info.ActiveFrameIndex + 1;
            }

            //到达结尾、并且是循环，那么返回头部
            if (_info.ActiveFrameIndex == _info.ActiveAction.actionFrame.aniFrameInfos.Count - 1
                && _info.ActiveAction.loopType == ActionLoopType.Loop)
            {
                return 0;
            }

            //返回错误
            return ActionConstant.InvalidFrameIndex;
        }

        #endregion

        #endregion

        #region 行为状态机-外部接口

        //内部打断-行为：按键、未按键、未满足条件
        //内部打断-帧：按键、未按键、未满足条件
        //外部打断-行为：伤害、控制、强制、未满足条件等

        #endregion

        #endregion

        #region 朝向处理

        //朝向变更
        private void ChangeOrientation()
        {
            if (_info.ActiveAction == null)
            {
                return;
            }

            List<OrientationSwitchCondition> limits = _info.ActiveAction.actionFrame.orientationLimits;
            if (CollectionUtil.IsEmpty(limits))
            {
                return;
            }

            foreach (var item in limits)
            {
                switch (item.type)
                {
                    case OrientationSwitchType.Key:
                        OrientationSwitchKey(item);
                        break;
                }
            }
        }

        //朝向变更-按键
        private void OrientationSwitchKey(OrientationSwitchCondition condition)
        {
            //如果没有按键，不处理
            if (!Core.HasKeyCurrentFrame(condition.key.keyLimits))
            {
                return;
            }

            Core.ChangeOrientation(condition.key.orientation);
        }

        #endregion

        #region 事件发生

        //发送物体销毁事件-角色死亡、活跃物体&特效消失
        private void SendObjDeath()
        {
            OnObjDeath?.Invoke();
        }

        //发送行为变更事件-变更前
        private void SendActionChangePre(ActionSo preActionSo, ActionSo postActionSo)
        {
            ActionChangeEvent @event;
            @event.PreActionSo = preActionSo;
            @event.PostActionSo = postActionSo;
            OnActionPre?.Invoke(@event);
        }

        //发送行为变更事件-变更后
        private void SendActionChangePost(ActionSo preActionSo, ActionSo postActionSo)
        {
            ActionChangeEvent @event;
            @event.PreActionSo = preActionSo;
            @event.PostActionSo = postActionSo;
            OnActionPost?.Invoke(@event);
        }

        //发送帧变更事件-变更前
        private void SendFrameChangePre(int preFrameIndex, int postFrameIndex)
        {
            FrameChangeEvent @event;
            @event.PreIndex = preFrameIndex;
            @event.PostIndex = postFrameIndex;
            OnFramePre?.Invoke(@event);
        }

        //发送帧变更事件-变更后
        private void SendFrameChangePost(int preFrameIndex, int postFrameIndex)
        {
            FrameChangeEvent @event;
            @event.PreIndex = preFrameIndex;
            @event.PostIndex = postFrameIndex;
            OnFramePost?.Invoke(@event);
        }

        #endregion

        #region AbstractObjUnit 接口

        public override void Init(ObjCore core)
        {
            base.Init(core);
            HasInit = true;
        }

        public override void InnerUpdate()
        {
            //运行状态机
            RunStateMachine();
        }

        #endregion
    }
    
    #region 工具类

        //行为信息
        public class ActionInfo
        {
            //是否是AI控制
            public bool IsAI { get; private set; }

            //AI控制变更
            public bool AiChange { get; private set; }
            
            //AI的Action
            public ActionSo AiAction { get; private set; }
            
            //AI的帧序列
            public int AiFrameIndex { get; private set; }
            
            //行为联动相关信息
            public LinkageInfo Linkage { get; private set; }
            
            //上一次行为
            public ActionSo PreAction { get; private set; }

            //当前生效的行为
            public ActionSo ActiveAction { get; private set; }

            //上一次行为UUID：实时UUID，每次转换行为都会重新生成
            public string PreUuid { get; private set; }

            //当前行为UUID：实时UUID，每次转换行为都会重新生成
            public string ActiveUuid { get; private set; }

            //上一个帧
            public AniFrameInfo PreFrame { get; private set; }

            //上一个帧序列
            public int PreFrameIndex { get; private set; }

            //当前帧
            public AniFrameInfo ActiveFrame { get; private set; }

            //当前帧序列
            public int ActiveFrameIndex { get; private set; }

            //当前帧经过的时间，MS
            public float ActiveFrameTime { get; private set; }

            //当前行为经过的时间，MS
            public float ActiveActionTime { get; private set; }

            //是否已经在处理顿帧
            public bool OnFreezeProcess { get; private set; }
            
            //该帧是否已经处理顿帧结束
            public bool HasFreezeOver { get; private set; }
            
            //顿帧经过的时间
            public float FreezeLastTime { get; private set; }

            public static ActionInfo BuildInit()
            {
                ActionInfo info = new ActionInfo();
                info.IsAI = false;
                info.AiChange = false;
                info.AiAction = null;
                info.AiFrameIndex = ActionConstant.InvalidFrameIndex;
                info.Linkage = LinkageInfo.BuildInit();
                info.PreAction = null;
                info.ActiveAction = null;
                info.PreUuid = null;
                info.ActiveUuid = null;
                info.PreFrame = null;
                info.PreFrameIndex = ActionConstant.InvalidFrameIndex;
                info.ActiveFrame = null;
                info.ActiveFrameIndex = ActionConstant.InvalidFrameIndex;
                info.ActiveFrameTime = 0f;
                info.ActiveActionTime = 0f;
                info.OnFreezeProcess = false;
                info.HasFreezeOver = false;
                info.FreezeLastTime = 0f;
                return info;
            }

            //开启顿帧
            public void FreezeStart()
            {
                OnFreezeProcess = true;
            }

            //完成顿帧
            public void FreezeOver()
            {
                OnFreezeProcess = false;
                HasFreezeOver = true;
            }

            //变更为AI控制
            public void ChangeToAi(ActionSo action, int frameIndex)
            {
                IsAI = true;
                AiChange = true;
                AiAction = action;
                AiFrameIndex = frameIndex;
            }

            //变更为手动控制
            public void ChangeToManual()
            {
                IsAI = false;
                AiChange = true;
                AiAction = null;
                AiFrameIndex = ActionConstant.InvalidFrameIndex;
            }

            //取消Ai变更标识
            public void AiTagCancel()
            {
                AiChange = false;
            }

            //帧前处理
            public void UpdatePre()
            {
                //帧处理
                ActiveFrameTime += TimeUtil.DeltaTimeMs();
                ActiveActionTime += TimeUtil.DeltaTimeMs();
                
                //顿帧处理
                if (OnFreezeProcess)
                {
                    FreezeLastTime += TimeUtil.DeltaTimeMs();
                }
                else
                {
                    OnFreezeProcess = false;
                    HasFreezeOver = false;
                    FreezeLastTime = 0f;
                }

                if (!Linkage.HasLinkage)
                {
                    return;
                }
                
                //联动处理
                //检查是否需要清理联动信息
                bool needClear = false;
                switch (Linkage.Linkage.condition.type)
                {
                    case LinkageSwitchType.Auto:
                        break;
                    case LinkageSwitchType.TimeControlLinear:
                        if (Linkage.Linkage.condition.timeLimitSame.startFrame <= ActiveFrameIndex)
                        {
                            Linkage.Time += TimeUtil.DeltaTimeMs();
                        }
                        needClear = Linkage.Time > Linkage.Linkage.condition.timeLimitSame.time;
                        break;
                }

                if (needClear)
                {
                    ClearLinkage();
                }
            }

            //切换到某个行为
            public void ChangeAction(ActionSo action)
            {
                PreAction = ActiveAction;
                PreUuid = ActiveUuid;
                ActiveAction = action;
                ActiveUuid = UuidUtil.Uuid();
            }

            //初始化帧
            public void InitFrame()
            {
                PreFrameIndex = ActionConstant.InvalidFrameIndex;
                PreFrame = null;

                ActiveFrameIndex = 0;
                ActiveFrame = ActiveAction.actionFrame.aniFrameInfos[0];
                ActiveFrameTime = 0f;
                ActiveActionTime = 0f;
                OnFreezeProcess = false;
                HasFreezeOver = false;
                FreezeLastTime = 0f;
            }

            //下一帧
            public void NextFrame(int index)
            {
                PreFrameIndex = ActiveFrameIndex;
                PreFrame = ActiveFrame;

                ActiveFrameIndex = index;
                ActiveFrame = ActiveAction.actionFrame.aniFrameInfos[index];
                ActiveFrameTime = 0f;
                OnFreezeProcess = false;
                HasFreezeOver = false;
                FreezeLastTime = 0f;
            }

            //重置到第一帧
            public void ResetFrame()
            {
                PreFrameIndex = ActiveFrameIndex;
                PreFrame = ActiveFrame;

                ActiveFrameIndex = 0;
                ActiveFrame = ActiveAction.actionFrame.aniFrameInfos[0];
                ActiveFrameTime = 0f;
                OnFreezeProcess = false;
                HasFreezeOver = false;
                FreezeLastTime = 0f;
            }

            #region 联动相关方法

            //写入并初始化一个联动
            public void SetLinkage(ActionSo action)
            {
                if (null == action.actionOther.linkage.linkageAction
                    || null == action.actionOther.linkage.condition)
                {
                    return;
                }
                
                Linkage.HasLinkage = true;
                //如果是自动，那么默认可以切换到联动行为
                Linkage.CanChangeLinkage = action.actionOther.linkage.condition.type == LinkageSwitchType.Auto;
                Linkage.PreAction = action;
                Linkage.PostAction = action.actionOther.linkage.linkageAction;
                Linkage.Linkage = action.actionOther.linkage;
                Linkage.Time = 0f;
            }

            //清除联动信息
            public void ClearLinkage()
            {
                Linkage.HasLinkage = false;
                Linkage.CanChangeLinkage = false;
                Linkage.PreAction = null;
                Linkage.PostAction = null;
                Linkage.Linkage = null;
                Linkage.Time = 0f;
            }

            //是否需要切换到联动行为
            public bool CanChangeLinkage(ActionSo action)
            {
                //没有联动、或联动已经处于激活状态，那么不处理
                if (!Linkage.HasLinkage || Linkage.CanChangeLinkage)
                {
                    return false;
                }

                switch (Linkage.Linkage.condition.type)
                {
                    //自动
                    case LinkageSwitchType.Auto:
                        return true;
                    //时间&相同行为类型
                    case LinkageSwitchType.TimeControlLinear:
                        return ActiveFrameIndex >= Linkage.Linkage.condition.timeLimitSame.startFrame
                               && Linkage.Time < Linkage.Linkage.condition.timeLimitSame.time
                               && action.type == Linkage.PostAction.type;
                }

                return false;
            }
            
            //设置联动有效，在当前行为结束后，会自动切换到联动行为上
            public void ActiveLinkage()
            {
                if (!Linkage.HasLinkage)
                {
                    return;
                }
                Linkage.CanChangeLinkage = true;
                Log.DebugInfo("[ActionUnit]联动激活。", LogModule.ObjCore);
            }

            #endregion
        }

        //行为联动信息
        public class LinkageInfo
        {
            //是否有联动
            public bool HasLinkage;
            //是否能够切换到联动行为
            public bool CanChangeLinkage;
            //之前的行为
            public ActionSo PreAction;
            //待联动的行为
            public ActionSo PostAction;
            //联动条件，隶属于PreAction
            public Linkage Linkage;
            
            //联动限制中已经经过的时间
            public float Time;

            public static LinkageInfo BuildInit()
            {
                LinkageInfo info = new LinkageInfo();
                info.HasLinkage = false;
                info.CanChangeLinkage = false;
                info.PreAction = null;
                info.PostAction = null;
                info.Linkage = null;
                info.Time = 0f;
                return info;
            }
        }

        #endregion
}