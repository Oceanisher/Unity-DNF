using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using Core;
using JetBrains.Annotations;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Control
{
    //动画处理单元
    public class GraphicsProcessor : MonoBehaviour
    {
        //动画完成回调
        public UnityAction OnBehaviourOver;

        //子节点所有的精灵单元
        private SpriteUnit [] _childSpriteUnitList;
        
        //行为
        private BehaviourConfigSo _behaviour;

        //当前动画对应的帧
        private AniFrame? _frame;

        //当前帧已经持续的时间
        private float _frameDuration;

        //当前帧的位置
        private int _frameSequence;
        
        //父节点ObjSupervisor组件
        private ObjSupervisor _parentSupervisor;
        
        private void Start()
        {
            _parentSupervisor = GetComponentInParent<ObjSupervisor>();
            _childSpriteUnitList = GetComponentsInChildren<SpriteUnit>();
            
            //开局初始化
            InitFrame();
        }

        private void Update()
        {
            //更新帧
            UpdateFrame();
        }
        
        //转换行为
        public void ChangeBehaviour(BehaviourConfigSo behaviour)
        {
            _behaviour = behaviour;
            ResetFrame();
        }

        #region 帧更新

        //更新帧
        private void UpdateFrame()
        {
            //获取当前帧配置
            if (null == _frame)
            {
                ResetFrame();
            }

            if (null == _frame)
            {
                return;
            }

            //如果当前帧已经超过该帧应该的时间，就切换；否则，持续该帧
            if (_frameDuration >= _frame?.aniFrameDuration)
            {
                NextFrame();
            }
            else
            {
                ContinueFrame();
            }
        }

        //初始化帧
        private void InitFrame()
        {
            _frame = null;
            _frameDuration = 0f;
            _frameSequence = 0;
        }

        //重置帧
        private void ResetFrame()
        {
            if (null != _behaviour)
            {
                SetFrame(0);
            }
        }

        //切换帧
        private void NextFrame()
        {
            
            if (_frameSequence >= _behaviour.aniSequence.Count - 1)
            {
                //如果是循环动画，那么重复播放
                if (_behaviour.controlType == BehaviourControlType.Sustain
                    || _behaviour.controlType == BehaviourControlType.SustainInverse)
                {
                    ResetFrame();
                }
                //否则回调已经完成
                else
                {
                    OnBehaviourOver?.Invoke();
                }
                return;
            }

            SetFrame(_frameSequence + 1);
        }

        //持续帧
        private void ContinueFrame()
        {
            _frameDuration += TimeUtil.DeltaTimeMs();
        }

        //设置帧
        private void SetFrame(int sequence)
        {
            _frame = _behaviour.aniSequence[sequence];
            _frameSequence = sequence;
            _frameDuration = 0f;
            SetSprite();
        }

        //写入精灵
        private void SetSprite()
        {
            if (null == _frame || null == _behaviour)
            {
                return;
            }

            if (null == _childSpriteUnitList || _childSpriteUnitList.Length == 0)
            {
                return;
            }

            foreach (var unit in _childSpriteUnitList)
            {
                unit.ChangeSprite(int.Parse(_frame.Value.aniFrameSprite));
            }
        }

        #endregion
    }
}