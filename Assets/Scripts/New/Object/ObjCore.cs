using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using New.Object.Config.Property;
using New.Object.Event;
using New.Object.Unit;
using New.Object.Unit.Control;
using New.Object.Unit.UI;
using New.Tools;
using Tools;
using UnityEngine;
using GameManager = New.Core.GameManager;
using InputManager = New.Core.Module.InputManager;
using KeyType = New.Core.Config.KeyType;

namespace New.Object
{
    //物体核心
    public class ObjCore : MonoBehaviour, IInit
    {
        //是否初始化完成
        public bool HasInit { get; set; }
        
        [Header("物体配置")]
        [SerializeField]
        private ObjConfigSo objConfigSo;
        public ObjConfigSo ObjConfigSo => objConfigSo;
        public GlobalGraphics GlobalGraphics => objConfigSo.graphics;
        public ActionSo TownDefaultAction => objConfigSo.townDefaultAction;
        public ActionSo FightDefaultGroundAction => objConfigSo.fightDefaultGroundAction;
        public ActionSo FightDefaultFlyAction => objConfigSo.fightDefaultFlyAction;
        public ActionSo SkyStayAction => objConfigSo.skyStayAction;
        public ActionSo GroundHurtAction => objConfigSo.groundHurtAction;
        public ActionSo GroundControlledAction => objConfigSo.groundControlledAction;
        public ActionSo SkyHurtAction => objConfigSo.skyHurtAction;
        public ActionSo SkyControlledAction => objConfigSo.skyControlledAction;
        public ActionSo CollapseAction => objConfigSo.collapseAction;
        public ActionSo FallAction => objConfigSo.fallAction;
        public ActionSo GetUpAction => objConfigSo.getUpAction;
        public List<ActionSo> SkillActions => objConfigSo.skillActions;

        //物体属性配置
        private ObjProperties _properties;
        public ObjProperties Properties => _properties;
        public InherentProperties InherentProperties => _properties.InherentProperties;
        public DynamicProperties DynamicProperties => _properties.DynamicProperties;
        public SpecialProperties SpecialProperties => _properties.SpecialProperties;
        
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
        
        //初始化
        public void Init()
        {
            _actionUnit = GetComponentInChildren<ActionUnit>();
            _displacementUnit = GetComponentInChildren<DisplacementUnit>();
            _physicsUnit = GetComponentInChildren<PhysicsUnit>();
            _graphicsUnit = GetComponentInChildren<GraphicsUnit>();

            _unitList = new List<AbstractObjUnit>();
            _unitList.Add(_actionUnit);
            _unitList.Add(_displacementUnit);
            _unitList.Add(_physicsUnit);
            _unitList.Add(_graphicsUnit);
            
            _actionUnit.OnActionPre += OnActionChangePre;
            _actionUnit.OnActionPost += OnActionChangePost;
            _actionUnit.OnFramePre += OnFrameChangePre;
            _actionUnit.OnFramePost += OnFrameChangePost;

            _properties = objConfigSo.GetProperties();
            
            InitUnit();

            //完成初始化
            HasInit = true;
        }

        //初始化Unit
        private void InitUnit()
        {
            foreach (var unit in _unitList)
            {
                unit.Init(this);
            }
        }

        #region 外部方法-写

        //接受按键处理
        public void ReceiveKeyInfos(List<InputManager.KeyInfo> keyInfos)
        {
            if (!HasInit)
            {
                return;
            }
            
            if (InherentProperties.AllowKeyInput)
            {
                _actionUnit.ReceiveKeyInfos(keyInfos);
            }
        }

        #endregion

        #region 外部方法

        //获取场景类型
        public SceneType GetSceneType()
        {
            return GameManager.Instance.SceneType;
        }

        //变更角色位置类型
        public void ChangeObjPosState(ObjPosState state)
        {
            ObjPosState oldState = _properties.DynamicProperties.CharacterProperties.PosState;
            _properties.DynamicProperties.CharacterProperties.PosState = state;
            
            //发送事件
            SendObjPosState(oldState, state);
        }

        #endregion

        #region 工具方法-读
        
        //获取当前行为的帧配置
        public AniFrameInfo GetActiveFrameInfo()
        {
            return _actionUnit.ActiveFrameInfo;
        }

        #endregion

        #region 事件发出

        //发送角色位置变更事件
        public void SendObjPosState(ObjPosState oldState, ObjPosState newState)
        {
            //TODO
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

        #endregion
    }
}