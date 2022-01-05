using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using New.Object.Config.Property;
using New.Object.Event;
using New.Tools;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using InputManager = New.Core.Module.InputManager;

namespace New.Object.Unit.Control
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
        public ActionSo ActiveAction => _activeAction;
        //当前帧序列
        public int ActiveFrameIndex => _activeFrameIndex;
        //当前帧信息
        public AniFrameInfo ActiveFrameInfo => _activeFrame;

        #endregion

        //上一次行为
        private ActionSo _preAction;
        //当前生效的行为
        private ActionSo _activeAction;
        
        //上一个帧
        private AniFrameInfo _preFrame;
        //上一个帧序列
        private int _preFrameIndex;
        //当前帧
        private AniFrameInfo _activeFrame;
        //当前帧序列
        private int _activeFrameIndex;
        //当前帧经过的时间，MS
        private float _activeFrameTime;
        //当前行为经过的时间，MS
        private float _activeActionTime;
        
        //最近的按键列表
        private List<InputManager.KeyInfo> _lastKeyInfos;

        private void Update()
        {
            RunStateMachine();
        }

        // private void LateUpdate()
        // {
        //     RunStateMachine();
        // }

        #region 外部开放接口

        //接受按键处理
        public void ReceiveKeyInfos(List<InputManager.KeyInfo> keyInfos)
        {
            _lastKeyInfos = keyInfos;
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
            if (null == _activeAction)
            {
                // Log.DebugInfo("[ActionUnit]行为变更-初始化。");
                ChangeAction(GetDefaultAction());
                return;
            }
            
            //按键变更检查
            if (ActionChangeCheck_Key(out var changeAction))
            {
                // Log.DebugInfo($"[ActionUnit]行为变更-按键。action:{changeAction.actionName}");
                ChangeAction(changeAction);
                return;
            }

            //如果没有其他行为，那么处理当前行为
            // Log.DebugInfo($"[ActionUnit]行为持续。action:{_activeAction.actionName}");
            HandleCurrent();
        }

        //行为变更检查-有无按键
        private bool ActionChangeCheck_Key(out ActionSo changeAction)
        {
            if (CollectionUtil.IsEmpty(_lastKeyInfos))
            {
                changeAction = null;
                return false;
            }

            ActionSo actionSo = Core.ObjConfigSo.GetActionByKeys(_lastKeyInfos);
            
            //如果行为是当前行为，那么不改变
            if (_activeAction == actionSo)
            {
                changeAction = null;
                return false;
            }
            
            changeAction = actionSo;
            return null != actionSo;
        }

        //处理当前行为
        private void HandleCurrent()
        {
            //如果当前行为正常、未被打断，那么正常变更帧
            FrameCheckResult checkResult = ActionContinueCheck();
            
            // Log.DebugInfo($"[ActionUnit]当前行为处理检查。action:{_activeAction.actionName}, 检查结果:{checkResult}");

            //TODO 如果不能继续，那么回到默认行为。实际上要看能回到哪个行为，空中、陆地、或者指定的新行为
            switch (checkResult)
            {
                //保持该帧，不做任何事情
                case FrameCheckResult.FrameKeep:
                    break;
                //下一帧
                case FrameCheckResult.FrameNext:
                    NextFrame(_activeFrameIndex + 1);
                    break;
                //重置帧，重置到第0帧
                case FrameCheckResult.FrameReset:
                    InitFrame();
                    break;
                //打断
                case FrameCheckResult.ActionTerminate:
                    ChangeAction(GetDefaultAction());
                    break;
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
            _activeFrameTime += TimeUtil.DeltaTimeMs();
            _activeActionTime += TimeUtil.DeltaTimeMs();
        }
        
        //状态机帧后处理
        private void PostRunStateMachine()
        {
            if (!CollectionUtil.IsEmpty(_lastKeyInfos))
            {
                _lastKeyInfos.Clear();
            }
        }

        //获取默认行为
        private ActionSo GetDefaultAction()
        {
            //非角色类，直接返回城镇默认
            if (ObjType.Character != Core.InherentProperties.ObjType)
            {
                return Core.TownDefaultAction;
            }
            
            //角色类要根据场景、角色类型判断
            switch (Core.GetSceneType())
            {
                //默认返回城镇默认行为
                case SceneType.None:
                    return Core.TownDefaultAction;
                case SceneType.Town:
                    return Core.TownDefaultAction;
                case SceneType.Fight:
                    switch (Core.InherentProperties.CharacterInherentProperties.FlyType)
                    {
                        //默认返回战斗陆地行为
                        case FlyType.None:
                            return Core.FightDefaultGroundAction;
                        case FlyType.Ground:
                            return Core.FightDefaultGroundAction;
                        case FlyType.Fly:
                            return Core.FightDefaultFlyAction;
                        //混合类默认返回飞行行为
                        case FlyType.GroundFly:
                            return Core.FightDefaultFlyAction;
                    }
                    break;
            }

            return Core.TownDefaultAction;
        }
        
        //当前帧是否有指定按键
        private bool HasKeyCurrentFrame(List<KeyLimit> keyLimits)
        {
            if (CollectionUtil.IsEmpty(_lastKeyInfos))
            {
                return false;
            }

            foreach (var limit in keyLimits)
            {
                //如果是任意按键，那么返回true
                if (limit.timeKey == KeyType.Any)
                {
                    return true;
                }

                foreach (var keyInfo in _lastKeyInfos)
                {
                    if (limit.timeKey == keyInfo.KeyType
                        && limit.timeKeyStateList.Contains(keyInfo.KeyState))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //行为变更
        private void ChangeAction(ActionSo actionSo)
        {
            //变更前
            SendActionChangePre(_preAction, actionSo);

            //执行变更
            ChangeActionInner(actionSo);
            
            //变更后
            SendActionChangePost(_preAction, _activeAction);
            
            //初始化帧
            InitFrame();
        }

        //行为变更，实际执行
        private void ChangeActionInner(ActionSo actionSo)
        {
            _preAction = _activeAction;
            _activeAction = actionSo;
        }

        //能否继续进行本行为检查
        private FrameCheckResult ActionContinueCheck()
        {
            //无条件，是错误的配置，那么返回None
            if (CollectionUtil.IsEmpty(_activeAction.actionFrame.switchLimits)
                && _activeAction.loopType == ActionLoopType.Loop)
            {
                return FrameCheckResult.None;
            }

            //是否能够改变行为
            CanChangeActionInner(out var canChangeAction);
            if (canChangeAction)
            {
                return FrameCheckResult.ActionTerminate;
            }
            
            //是否能够切换到下一帧
            CanChangeFrameInner(out var isEnd, out var canChangeFrame);
            //行为结束、帧结束：行为终结
            //行为结束、帧未结束：说明是循环行为，重置到第一帧
            //行为未结束、帧结束：说明需要下一帧
            //行为未结束、帧未结束：说明需要保持该帧
            return isEnd ?
                (canChangeFrame ? FrameCheckResult.FrameReset : FrameCheckResult.ActionTerminate) : 
                (canChangeFrame ? FrameCheckResult.FrameNext : FrameCheckResult.FrameKeep);
        }

        //是否需要变更行为
        private void CanChangeActionInner(out bool result)
        {
            result = false;
            foreach (var item in _activeAction.actionFrame.switchLimits)
            {
                //只处理切换行为类型、只处理当前帧需要的限制
                if (item.switchOptType != SwitchOptType.BreakAction
                    || !item.frameIndexes.Contains(_activeFrameIndex))
                {
                    continue;
                }

                switch (item.switchType)
                {
                    case SwitchType.Key:
                        if (HasKeyCurrentFrame(item.timeKeys))
                        {
                            result = true;
                        }
                        break;
                    case SwitchType.KeyInvalid:
                        if (!HasKeyCurrentFrame(item.timeKeys))
                        {
                            result = true;
                        }
                        break;
                    case SwitchType.Time:
                        if (_activeActionTime >= item.time)
                        {
                            result = true;
                        }
                        break;
                    case SwitchType.TimeKey:
                        if (_activeActionTime >= item.time || HasKeyCurrentFrame(item.timeKeys))
                        {
                            result = true;
                        }
                        break;
                    case SwitchType.TimeKeyInvalid:
                        if (_activeActionTime >= item.time || !HasKeyCurrentFrame(item.timeKeys))
                        {
                            result = true;
                        }                       
                        break;
                    case SwitchType.ConditionHit:
                        break;
                    case SwitchType.ConditionHorizontal:
                        break;
                    case SwitchType.ConditionVertical:
                        break;
                    case SwitchType.ConditionLocalHorizontal:
                        break;
                    case SwitchType.ConditionLocalVertical:
                        break;
                }
            }
        }

        //是否能够继续帧
        private void CanChangeFrameInner(out bool isEnd, out bool canChange)
        {
            canChange = false;
            isEnd = false;
            
            //判断当前帧是否能切换
            //根据帧类型做判断，只要有一个为true即可
            foreach (var item in _activeAction.actionFrame.switchLimits)
            {
                //只处理切换帧类型、只处理当前帧需要的限制
                if (item.switchOptType != SwitchOptType.NextFrame 
                    || !item.frameIndexes.Contains(_activeFrameIndex))
                {
                    continue;
                }
                
                switch (item.switchType)
                {
                    case SwitchType.Time:
                        if (_activeFrameTime >= item.time)
                        {
                            canChange = true;
                        }
                        break;
                    case SwitchType.TimeKey:
                        if (_activeFrameTime >= item.time || HasKeyCurrentFrame(item.timeKeys))
                        {
                            canChange = true;
                        }
                        break;
                    case SwitchType.TimeKeyInvalid:
                        if (_activeFrameTime >= item.time || !HasKeyCurrentFrame(item.timeKeys))
                        {
                            canChange = true;
                        }
                        break;
                    case SwitchType.ConditionHit:
                        break;
                    case SwitchType.ConditionHorizontal:
                        break;
                    case SwitchType.ConditionVertical:
                        break;
                    case SwitchType.ConditionLocalHorizontal:
                        break;
                    case SwitchType.ConditionLocalVertical:
                        break;
                }
            }

            //如果可以切换帧，那么还需要判断是否能够继续播放
            if (canChange)
            {
                //未到达最后一帧
                if (_activeFrameIndex < _activeAction.actionFrame.aniFrameInfos.Count - 1)
                {
                    isEnd = false;
                    return;
                }
                
                //到达最后一帧，如果是循环行为，那么可以自动到第一帧
                if (ActionLoopTypeExtend.IsLoop(_activeAction.loopType))
                {
                    isEnd = true;
                    return;
                }
                //否则，结束行为
                isEnd = true;
                return;
            }
        }

        //初始化帧
        private void InitFrame()
        {
            //变更前
            SendFrameChangePre(_activeFrameIndex, ActionConstant.InvalidFrameIndex);
            //执行变更
            InitFrameInner();
            //变更后
            SendFrameChangePost(ActionConstant.InvalidFrameIndex, _activeFrameIndex);
        }
        
        //初始化帧，实际执行
        private void InitFrameInner()
        {
            _preFrameIndex = ActionConstant.InvalidFrameIndex;
            _preFrame = null;
            
            _activeFrameIndex = 0;
            _activeFrame = _activeAction.actionFrame.aniFrameInfos[0];
            _activeFrameTime = 0f;
            _activeActionTime = 0f;
        }

        //下一帧
        private void NextFrame(int index)
        {
            //变更前
            SendFrameChangePre(_activeFrameIndex, index);
            //执行变更
            NextFrameInner(index);
            //变更后
            SendFrameChangePost(_preFrameIndex, _activeFrameIndex);
        }
        
        //下一帧，实际执行
        private void NextFrameInner(int index)
        {
            _preFrameIndex = _activeFrameIndex;
            _preFrame = _activeFrame;
            
            _activeFrameIndex = index;
            _activeFrame = _activeAction.actionFrame.aniFrameInfos[index];
            _activeFrameTime = 0f;
        }

        #endregion
        
        #region 行为状态机-外部接口
        
        //内部打断-行为：按键、未按键、未满足条件
        //内部打断-帧：按键、未按键、未满足条件
        //外部打断-行为：伤害、控制、强制、未满足条件等
        
        #endregion

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

        #endregion

        //帧检测结果
        private enum FrameCheckResult
        {
            None,//无
            FrameKeep,//保持当前帧
            FrameNext,//进行下一帧
            FrameReset,//重置到第一帧
            ActionTerminate,//结束行为
        }
    }
}