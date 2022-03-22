using System;
using System.Collections.Generic;
using Sys.Config;
using UnityEngine;

namespace Sys.Module
{
    //全局物料管理器
    public class GlobalResManager : AbstractManager<GlobalResManager>
    {
        [Header("碰撞单元组件")]
        [SerializeField]
        private GameObject colComponent;
        public GameObject ColComponent => colComponent;

        [Header("被击中时的音效，随机播放")]
        [SerializeField]
        private List<AudioClip> beHitAudioClips;
        public List<AudioClip> BeHitAudioClips => beHitAudioClips;
        
        [Header("被击中音频音量百分比，0~100")]
        [SerializeField]
        private int beHitVolumePercent;
        public int BeHitVolumePercent => beHitVolumePercent;
    }
}