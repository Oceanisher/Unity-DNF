using System;
using System.Collections.Generic;
using System.Linq;
using Obj.Config.Action.Structure;
using Obj.Event;
using Obj.Unit.Control;
using Sys.Module;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obj.Unit.UI
{
    //音频单元
    public class AudioUnit : AbstractObjUnit
    {
        //主动音频源
        private AudioSource _activeAs;
        //被动音频源
        private AudioSource _passiveAs;
        //帧音频配置与音频选择映射，用于顺序播放音频的情况
        private Dictionary<FrameAudioInfo, int> _frameAudioIndexMap;

        #region 外部接口

        //被击中音效（例如受击等）
        public void OnFightPassive(FightInfo fightInfo)
        {
            HandleBeHitAudio();
        }

        #endregion

        #region 被动音频处理

        //受击音频处理
        private void HandleBeHitAudio()
        {
            List<AudioClip> audioClips = GlobalResManager.Instance.BeHitAudioClips;
            if (CollectionUtil.IsEmpty(audioClips))
            {
                return;
            }

            AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];
            
            //设置音量
            _passiveAs.volume = GlobalResManager.Instance.BeHitVolumePercent / 100f;
            //播放
            _passiveAs.clip = clip;
            _passiveAs.Play();
        }

        #endregion
        
        #region 主动音频处理
        
        //处理行为改变
        private void HandleActionChange()
        {
            _frameAudioIndexMap.Clear();
        }

        //处理音频播放
        private void HandleAudio(int frameIndex)
        {
            if (!HasInit)
            {
                return;
            }
            
            //查找配置
            FrameAudioInfo audioInfo = GetAudioInfo(frameIndex);
            
            //播放
            if (null != audioInfo)
            {
                PlayAudio(audioInfo);
            }
        }

        //查找音频
        private FrameAudioInfo GetAudioInfo(int frameIndex)
        {
            List<FrameAudioInfo> audioConfigList = Core.GetActiveAction().actionFrame.frameAudioInfos;
            if (CollectionUtil.IsEmpty(audioConfigList))
            {
                return null;
            }

            foreach (var item in audioConfigList)
            {
                if (item.frameIndexes.Contains(frameIndex))
                {
                    return item;
                }
            }

            return null;
        }

        //播放音频
        private void PlayAudio(FrameAudioInfo frameAudioInfo)
        {
            if (CollectionUtil.IsEmpty(frameAudioInfo.audioClipList))
            {
                return;
            }
            
            //设置循环类型
            if (frameAudioInfo.loopType == FrameAudioLoopType.Once || frameAudioInfo.loopType == FrameAudioLoopType.None)
            {
                _activeAs.loop = false;
            }
            else if (frameAudioInfo.loopType == FrameAudioLoopType.LoopFrame)
            {
                _activeAs.loop = true;
            }
            
            //设置音量
            _activeAs.volume = frameAudioInfo.volumePercent / 100f;
            
            //播放
            _activeAs.clip = ChooseClip(frameAudioInfo);
            _activeAs.Play();
        }

        //选择音频片段
        private AudioClip ChooseClip(FrameAudioInfo frameAudioInfo)
        {
            if (frameAudioInfo.audioClipList.Count == 1)
            {
                return frameAudioInfo.audioClipList[0];
            }

            switch (frameAudioInfo.chooseType)
            {
                case FrameAudioChooseType.None:
                    return frameAudioInfo.audioClipList[0];
                case FrameAudioChooseType.Random:
                    return frameAudioInfo.audioClipList[Random.Range(0, frameAudioInfo.audioClipList.Count)];
                case FrameAudioChooseType.Order:
                    return frameAudioInfo.audioClipList[GetAndSetAudioIndex(frameAudioInfo)];
            }

            return null;
        }

        //获取并设置映射
        private int GetAndSetAudioIndex(FrameAudioInfo frameAudioInfo)
        {
            //不包含，那么写入
            if (!_frameAudioIndexMap.ContainsKey(frameAudioInfo))
            {
                _frameAudioIndexMap.Add(frameAudioInfo, 0);
                return 0;
            }
            
            //包含，那么+1，到头则归零
            _frameAudioIndexMap.TryGetValue(frameAudioInfo, out var index);
            _frameAudioIndexMap.Remove(frameAudioInfo);
            
            if (index + 1 >= frameAudioInfo.audioClipList.Count)
            {
                _frameAudioIndexMap.Add(frameAudioInfo, 0);
                return 0;
            }
            
            _frameAudioIndexMap.Add(frameAudioInfo, index + 1);
            return index + 1;
        }

        #endregion

        #region AbstractObjUnit 接口
        
        public override void Init(ObjCore objCore)
        {
            base.Init(objCore);

            _activeAs = GetComponent<AudioSource>();
            _passiveAs = Core.GetPhysicsGo().GetComponent<AudioSource>();

            if (null == _activeAs || null == _passiveAs)
            {
                Log.Error($"[AudioUnit]未配置主动/被动AudioSource组件。Go:{gameObject.name}", LogModule.ObjCore);
                return;
            }
            
            _frameAudioIndexMap = new Dictionary<FrameAudioInfo, int>();

            HasInit = true;
        }

        public override void OnActionChangePost(ActionChangeEvent changeEvent)
        {
            HandleActionChange();
        }

        public override void OnFrameChangePost(FrameChangeEvent changeEvent)
        {
            HandleAudio(changeEvent.PostIndex);
        }

        #endregion
    }
}