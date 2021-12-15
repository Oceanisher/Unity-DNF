using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using Core;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Control
{
    //物体管理器类
    public class ObjSupervisor : MonoBehaviour
    {
        [Header("角色类型")]
        [SerializeField]
        private CharacterType characterType;
        public CharacterType CharacterType => characterType;

        [Header("行为配置")]
        [SerializeField]
        private BehavioursConfigSo behavioursConfigSo;

        [Header("图形处理单元")]
        [SerializeField]
        [CanBeNull]
        private GraphicsProcessor gProcessor;
        
        [Header("物理处理单元")]
        [SerializeField]
        [CanBeNull]
        private PhysicsProcessor pProcessor;

        //朝向
        private OrientationType _orientation;
        
        //默认行为类型
        private BehaviourType _defaultType;
        //默认行为
        private BehaviourConfigSo _defaultBehaviour;
        
        //当前行为类型
        private BehaviourType _behaviourType = BehaviourType.StandTown;
        //当前行为
        private BehaviourConfigSo _behaviour = null;
        //当前行为是否继续，针对按键后才能继续的动画使用
        private bool _isSustainInverseContinue;
        //当前按键的信息
        private KeyInfo _key;
        //当前按键导致的行为持续时间，用于需要持续按键的行为控制
        private float _currentKeyTime;

        private void Start()
        {
            //初始化
            Init();
        }

        private void Update()
        {
            //持续按键行为检查检查
            SustainInverseCheck();
        }

        //持续按键行为检查检查
        private void SustainInverseCheck()
        {
            if (_behaviour.controlType != BehaviourControlType.SustainInverse)
            {
                return;
            }
            
            _currentKeyTime += TimeUtil.DeltaTimeMs();
            //如果当前时间已经持续超过了连续时间，那么需要切换
            if (_currentKeyTime >= CoreConstant.SustainKey)
            {
                ChangeAndProcessBehaviour(_defaultType);
            }
        }

        //转换行为
        //force：是否强制转换
        public void ChangeBehaviour(KeyInfo keyInfo)
        {
            //TODO
            _key = keyInfo;
            ChangeAndProcessBehaviour(keyInfo.BehaviourType);
        }

        //动画行为完成
        private void OnAniBehaviourOver()
        {
            //行为完成，默认回到默认行为
            ChangeAndProcessBehaviour(_defaultType);
        }

        //切换行为
        private void ChangeAndProcessBehaviour(BehaviourType type)
        {
            if (ChangeBehaviour(type))
            {
                ProcessBehaviour();
            }
        }
        
        //转换行为
        private bool ChangeBehaviour(BehaviourType type)
        {
            //TODO
            if (null == behavioursConfigSo)
            {
                return false;
            }
            BehaviourConfigSo behaviour = behavioursConfigSo.GetByType(type);
            if (null == behaviour)
            {
                return false;
            }
            
            //如果是不同行为，那么看是否可打断
            if (behaviour != _behaviour)
            {
                //TODO 目前可直接切换
                SwitchBehaviour(type, behaviour);
                return true;
            }
            
            SwitchBehaviour(type, behaviour);
            return false;
            
            // //如果是相同的行为
            // //对于需要持续按键才能持续的行为，需要判断是否是连续的
            // if (_behaviour.controlType == BehaviourControlType.SustainInverse)
            // {
            //     //如果按键不连续，那么切换到默认状态
            //     if (!_key.IsSustain)
            //     {
            //         SwitchBehaviour(_defaultType, _defaultBehaviour);
            //         return true;
            //     }
            // }

            // SwitchBehaviour(type, behaviour);
            // return true;
        }

        //切换行为
        private void SwitchBehaviour(BehaviourType type, BehaviourConfigSo behaviour)
        {
            _behaviourType = type;
            _behaviour = behaviour;
            
            //如果是持续按键行为，那么需要重置时间
            if (_behaviour.controlType == BehaviourControlType.SustainInverse)
            {
                _currentKeyTime = 0f;
            }
        }

        //处理行为
        private void ProcessBehaviour()
        {
            if (gProcessor)
            {
                gProcessor.ChangeBehaviour(_behaviour);
            }

            if (pProcessor)
            {
                //TODO
            }
        }

        //初始化
        private void Init()
        {
            //默认朝向向右
            _orientation = OrientationType.Right;
            
            if (null != behavioursConfigSo)
            {
                BehaviourConfigSo behaviour = behavioursConfigSo.GetByType(_behaviourType);
                if (null != behaviour)
                {
                    _behaviour = behaviour;
                    ProcessBehaviour();
                }

                //TODO 默认行为设置为城镇行为
                SetDefaultBehaviour(behavioursConfigSo.defaultBehaviourTown.type, behavioursConfigSo.defaultBehaviourTown);
            }

            if (gProcessor)
            {
                gProcessor.OnBehaviourOver += OnAniBehaviourOver;
            }
        }

        //设置默认行为
        private void SetDefaultBehaviour(BehaviourType type, BehaviourConfigSo behaviour)
        {
            _defaultBehaviour = behaviour;
            _defaultType = type;
        }
        
#if UNITY_EDITOR
        //绘制角色类身上的所有碰撞体
        private void OnDrawGizmos()
        {
            //绘制碰撞体框
            BoxCollider2D [] boxArray = GetComponentsInChildren<BoxCollider2D>();
            ObjGizmos.DrawActiveBox(boxArray);

            //绘制中心点圆圈
            GameObject mark = GameObject.Find("Mark");
            ObjGizmos.DrawCircle(mark);
        }
#endif
    }   
}
