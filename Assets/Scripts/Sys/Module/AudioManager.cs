using System;
using System.Collections.Generic;
using Sys.Config;
using UnityEngine;

namespace Sys.Module
{
    //音频中枢
    public class AudioManager : AbstractManager<AudioManager>
    {
        [Header("暂时默认的音频文件")]
        [SerializeField]
        private AudioClip defaultClip;
        [Header("暂时默认的音量")]
        [SerializeField]
        private int defaultVolume;

        //背景音频播放器
        private AudioSource _bgAs;

        //TODO 播放音频
        private void PlayAudio()
        {
            _bgAs.clip = defaultClip;
            _bgAs.loop = true;
            _bgAs.volume = defaultVolume / 100f;
            _bgAs.Play();
        }
        
        public override void Init(SceneType type)
        {
            base.Init(type);
            _bgAs = GetComponent<AudioSource>();

            HasInit = true;
            
            PlayAudio();
        }

        public override void ChangeScenePost(SceneType beforeType, SceneType afterType)
        {
            //TODO 暂时默认播放一个音频
            if (null == _bgAs || !_bgAs.isActiveAndEnabled || null == defaultClip)
            {
                return;
            }

            PlayAudio();
        }
    }
}