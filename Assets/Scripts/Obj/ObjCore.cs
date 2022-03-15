using System;
using System.Collections;
using System.Collections.Generic;
using Obj.Config;
using Obj.Config.Action;
using Obj.Config.Action.Structure;
using Obj.Config.Property;
using Obj.Event;
using Obj.Unit;
using Obj.Unit.Control;
using Obj.Unit.UI;
using Sys;
using Sys.Config;
using Sys.Module;
using Tools;
using UnityEngine;

namespace Obj
{
    //物体核心
    public class ObjCore : MonoBehaviour, IInit
    {
        //是否初始化完成
        public bool HasInit { get; set; }

        [Header("图形父节点")] [SerializeField] private GameObject _graphicsGo;
        [Header("物理父节点")] [SerializeField] private GameObject _physicsGo;
        [Header("物体配置")] [SerializeField] private ObjConfigSo objConfigSo;

        #region 属性/行为配置

        public ObjConfigSo ObjConfigSo => objConfigSo;
        public GlobalGraphics GlobalGraphics => objConfigSo.graphics;
        //行为字典
        private Dictionary<ActionType, ActionSo> _actionMap;

        public List<ActionSo> AllActions => objConfigSo.allActions;

        //物体属性配置
        private ObjProperties _properties;
        public ObjProperties Properties => _properties;
        public InherentProperties InherentProperties => _properties.InherentProperties;
        public DynamicProperties DynamicProperties => _properties.DynamicProperties;
        public SpecialProperties SpecialProperties => _properties.SpecialProperties;

        #endregion
        
        //行为单元
        private ActionUnit _actionUnit;

        //位移单元
        private DisplacementUnit _displacementUnit;

        //物理单元
        private PhysicsUnit _physicsUnit;

        //图形单元
        private GraphicsUnit _graphicsUnit;

        //单元列表
        private List<AbstractObjUnit> _unitList;

        //刚体
        private Rigidbody2D _rb;

        //最近的按键列表
        private List<InputManager.KeyInfo> _frameKeys = new List<InputManager.KeyInfo>();
        public List<InputManager.KeyInfo> FrameKeys => _frameKeys;

        //角色物体状态标志位
        private CharacterFlags _characterFlags;

        private void Update()
        {
            //单元更新
            UnitUpdate();
        }

        private void LateUpdate()
        {
            //单元Late更新
            UnitLateUpdate();
            //帧后处理
            PostFrame();
        }

        #region 外部方法-写

        //初始化
        public void Init()
        {
            //各类属性获取
            _actionUnit = GetComponentInChildren<ActionUnit>();
            _displacementUnit = GetComponentInChildren<DisplacementUnit>();
            _physicsUnit = GetComponentInChildren<PhysicsUnit>();
            _graphicsUnit = GetComponentInChildren<GraphicsUnit>();

            _unitList = new List<AbstractObjUnit>();
            _unitList.Add(_actionUnit);
            _unitList.Add(_displacementUnit);
            _unitList.Add(_physicsUnit);
            _unitList.Add(_graphicsUnit);
            
            _rb = GetComponent<Rigidbody2D>();
            
            //各类初始化
            ConfigInit();
            UnitInit();

            _actionUnit.OnActionPre += OnActionChangePre;
            _actionUnit.OnActionPost += OnActionChangePost;
            _actionUnit.OnFramePre += OnFrameChangePre;
            _actionUnit.OnFramePost += OnFrameChangePost;
            _displacementUnit.OnDisplacement += OnDisplacement;
            _physicsUnit.OnReColPost += OnReColPost;
            _physicsUnit.OnColEnter += OnColEnter;
            _physicsUnit.OnColExit += OnColExit;

            _characterFlags = CharacterFlags.BuildInit(Properties_IsFlyObj(), false);

            //完成初始化
            HasInit = true;
        }

        //接受按键处理
        public void ReceiveKeyInfos(List<InputManager.KeyInfo> keyInfos)
        {
            //不接受按键的物体跳过
            if (!HasInit || !InherentProperties.AllowKeyInput)
            {
                return;
            }

            _frameKeys.AddRange(keyInfos);
        }

        //变更角色位置类型
        public void ChangePosState(ObjPosState state)
        {
            ObjPosState preState = _properties.DynamicProperties.CharacterProperties.PosState;
            _properties.DynamicProperties.CharacterProperties.PosState = state;

            if (preState != state)
            {
                //发送事件
                SendPosStateEvent(preState, state);
            }
        }

        //变更物体朝向
        public void ChangeOrientation(Orientation orientation)
        {
            Orientation preOrientation = _properties.DynamicProperties.CharacterProperties.Orientation;
            _properties.DynamicProperties.CharacterProperties.Orientation = orientation;

            if (preOrientation != orientation)
            {
                //发送事件
                SendOrientationEvent(preOrientation, orientation);
            }
        }

        //AI行为变更
        public void ActionChange_Ai(ActionSo action, int frameIndex)
        {
            _actionUnit.ActionChange_AI(action, frameIndex);
        }

        //手动行为变更
        public void ActionChange_Manual()
        {
            _actionUnit.ActionChange_Manual();
        }

        #endregion

        #region 外部方法-读

        //获取行为
        public ActionSo GetAction(ActionType type)
        {
            _actionMap.TryGetValue(type, out var action);
            return action;
        }

        #endregion

        #region 工具方法-读

        //当前帧是否有指定按键
        public bool HasKeyCurrentFrame(List<KeyLimit> keyLimits)
        {
            if (CollectionUtil.IsEmpty(_frameKeys))
            {
                return false;
            }

            foreach (var limit in keyLimits)
            {
                //如果是任意按键，那么返回true
                if (limit.key == KeyType.Any)
                {
                    return true;
                }


                foreach (var keyInfo in _frameKeys)
                {
                    if (limit.key == keyInfo.KeyType
                        && limit.keyStateList.Contains(keyInfo.KeyState))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //获取刚体
        public Rigidbody2D GetRb()
        {
            return _rb;
        }

        //获取场景类型
        public SceneType GetSceneType()
        {
            return GameManager.Instance.SceneType;
        }

        //获取当前行为
        public ActionSo GetActiveAction()
        {
            return _actionUnit.ActiveAction;
        }

        //获取当前行为的当前帧序列
        public int GetActiveActionFrameIndex()
        {
            return _actionUnit.ActiveFrameIndex;
        }

        //获取当前行为的实时Uuid
        public string GetActiveActionUuid()
        {
            return _actionUnit.Info.ActiveUuid;
        }

        //获取当前行为的帧配置
        public AniFrameInfo GetActiveFrameInfo()
        {
            return _actionUnit.ActiveFrameInfo;
        }

        //获取当前行为的位移配置
        public ActionDisplacement GetActiveDisplacement()
        {
            return _actionUnit.ActiveAction.actionDisplacement;
        }

        //当前是否在顿帧中
        public bool GetActionFrameFreeze()
        {
            return _actionUnit.Info.OnFreezeProcess;
        }

        //获取图形父节点
        public GameObject GetGraphicsGo()
        {
            return _graphicsGo;
        }

        //获取物理父节点
        public GameObject GetPhysicsGo()
        {
            return _physicsGo;
        }

        //当前是否是正向
        public bool IsPositiveOrientation()
        {
            return objConfigSo.graphics.positiveOrientation ==
                   _properties.DynamicProperties.CharacterProperties.Orientation;
        }

        //默认的行走速度
        public Vector3 Properties_WalkSpeed()
        {
            return _properties.SpecialProperties.CharacterSpecialProperties.WalkSpeed;
        }
        
        //默认的跑步速度
        public Vector3 Properties_RunSpeed()
        {
            return _properties.SpecialProperties.CharacterSpecialProperties.RunSpeed;
        }

        //是否是飞行物体
        public bool Properties_IsFlyObj()
        {
            return _properties.InherentProperties.ObjType == ObjType.Character
                   && (_properties.InherentProperties.CharacterInherentProperties.FlyType == FlyType.Fly 
                       || _properties.InherentProperties.CharacterInherentProperties.FlyType == FlyType.GroundFly);
        }

        //是否在地面上
        public bool IsOnGround()
        {
            return ObjPosStateExtend.IsGround(_properties.DynamicProperties.CharacterProperties.PosState);
        }

        #endregion
        
        #region 工具方法-写
        
        //帧后处理
        private void PostFrame()
        {
            _frameKeys.Clear();
        }

        //配置文件初始化
        private void ConfigInit()
        {
            _properties = objConfigSo.GetProperties();
            _actionMap = new Dictionary<ActionType, ActionSo>();

            foreach (var item in objConfigSo.allActions)
            {
                //Hit行为只包含初始行为
                if (item.type == ActionType.CharacterHit)
                {
                    if (item.subType == ActionSubType.CharacterHit_1)
                    {
                        _actionMap.Add(item.type, item);
                    }
                }
                else
                {
                    _actionMap.Add(item.type, item);
                }
            }
        }

        //初始化Unit
        private void UnitInit()
        {
            foreach (var unit in _unitList)
            {
                unit.Init(this);
            }
        }

        //Unit的 InnerUpdate()调用
        private void UnitUpdate()
        {
            _actionUnit.InnerUpdate();
            _displacementUnit.InnerUpdate();
            _physicsUnit.InnerUpdate();
            _graphicsUnit.InnerUpdate();
        }
        
        //Unit的 InnerLateUpdate()调用
        private void UnitLateUpdate()
        {
            _actionUnit.InnerLateUpdate();
            _displacementUnit.InnerLateUpdate();
            _physicsUnit.InnerLateUpdate();
            _graphicsUnit.InnerLateUpdate();
        }
        
        #endregion

        #region 事件发出

        //发送物体位置变更事件
        public void SendPosStateEvent(ObjPosState pre, ObjPosState post)
        {
            //TODO
        }
        
        //发送物体朝向变更事件
        public void SendOrientationEvent(Orientation pre, Orientation post)
        {
            //TODO 目前只有GraphicsUnit关注朝向变更
            _graphicsUnit.OnOrientationChangePost();
        }
        
        #endregion

        #region 事件处理

        //行为变更-变更前
        private void OnActionChangePre(ActionChangeEvent @event)
        {
            foreach (var unit in _unitList)
            {
                unit.OnActionChangePre(@event);
            }
        }

        //行为变更-变更后
        private void OnActionChangePost(ActionChangeEvent @event)
        {
            foreach (var unit in _unitList)
            {
                unit.OnActionChangePost(@event);
            }
        }

        //帧变更-变更前
        private void OnFrameChangePre(FrameChangeEvent @event)
        {
            foreach (var unit in _unitList)
            {
                unit.OnFrameChangePre(@event);
            }
        }

        //帧变更-变更后
        private void OnFrameChangePost(FrameChangeEvent @event)
        {
            foreach (var unit in _unitList)
            {
                unit.OnFrameChangePost(@event);
            }
        }

        //位移回调
        private void OnDisplacement(DisplacementInfo displacementInfo)
        {
            //TODO 先回调Action单元
            _actionUnit.CharacterDisplacementChange(displacementInfo);
        }

        //重设碰撞器回调
        private void OnReColPost(PhysicsInfo info)
        {
        }

        //碰撞进入
        private void OnColEnter(ColItemInfo info)
        {
            //TODO 后续应该有DamageUnit来判断能够造成了伤害，才会调用ActionUnit去顿帧
            //被动碰撞方回调
            _actionUnit.ColEnterPassive(info);
            //主动碰撞方调用
            info.OtherCore._actionUnit.ColEnterActive(info);
        }
        
        //碰撞离开
        private void OnColExit(ColItemInfo info)
        {
            //TODO 暂无
        }

        #endregion

        #region 内部工具类

        //角色物体状态标志位
        private class CharacterFlags
        {
            /*******角色状态标识******/
            
            //是否地面
            public bool OnGround;
            //是否受击打断
            public bool Interrupted;
            //是否被控
            public bool Manipulated;
            //是否释放技能中
            public bool Skill;
            //是否倒地
            public bool Collapse;
            //起身中
            public bool OnGetUp;
            
            /*******其他细节标识******/
            
            //跳跃时到达最高点，跳跃时有效
            public bool ArriveHighest;

            //计算出角色当前的状态
            public ObjPosState GetState()
            {
                //地面-倒地中
                if (Collapse && !OnGetUp)
                {
                    return OnGround ? ObjPosState.GroundLie : ObjPosState.SkyLie;
                }

                //地面-起身中
                if (!Collapse && OnGetUp)
                {
                    return ObjPosState.GroundGetUp;
                }

                //地面-技能中 / 空中-技能中
                if (Skill)
                {
                    return OnGround ? ObjPosState.GroundSkill : ObjPosState.SkySkill;
                }

                //地面-被控中 / 空中-被控中
                if (Manipulated)
                {
                    return OnGround ? ObjPosState.GroundControlled : ObjPosState.SkyControlled;
                }

                //地面-受伤中 / 空中-受伤中
                if (Interrupted)
                {
                    return OnGround ? ObjPosState.GroundHurt : ObjPosState.SkyHurt;
                }
                
                //地面-正常 / 空中-正常
                return OnGround ? ObjPosState.Ground : ObjPosState.Sky;
            }

            public static CharacterFlags BuildInit(bool isFlyObj, bool isFly)
            {
                CharacterFlags flags = new CharacterFlags();
                flags.OnGround = !(isFlyObj && isFly);
                flags.Interrupted = false;
                flags.Manipulated = false;
                flags.Skill = false;
                flags.Collapse = false;
                flags.OnGetUp = false;
                return flags;
            }
        }

        #endregion
        
        #region Gizmos
        
#if UNITY_EDITOR

        //绘制碰撞器边框
        private void OnDrawGizmos()
        {
            if (null == _physicsUnit || !_physicsUnit.HasInit)
            {
                return;
            }
            
            //绘制静态
            if (null != _physicsUnit.Info.StaticCol && null != _physicsUnit.StaticXZ)
            {
                ColDrawUtil.DrawOjbColStaticXZ(_physicsUnit.StaticXZ, _physicsUnit.Info.StaticCol);
            }
            
            if (CollectionUtil.IsEmpty(_physicsUnit.Info.PhyColMap))
            {
                return;
            }
            
            //绘制动态
            foreach (var item in _physicsUnit.Info.PhyColMap)
            {
                switch (item.Key.colPos)
                {
                    case ColPos.None:
                        break;
                    case ColPos.XZ:
                        ColDrawUtil.DrawObjCol(_physicsUnit.DynamicXZ, item.Value, item.Key, _physicsUnit.Info.GetPhy(item.Key));
                        break;
                    case ColPos.Y:
                        ColDrawUtil.DrawObjCol(_physicsUnit.DynamicY, item.Value, item.Key, _physicsUnit.Info.GetPhy(item.Key));
                        break;
                }
            }
        }

#endif
        
        #endregion
    }
}