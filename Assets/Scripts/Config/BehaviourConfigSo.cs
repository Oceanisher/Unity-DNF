using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Config
{
    //行为配置
    [Obsolete]
    [CreateAssetMenu(menuName = "Config/Ani&Phy/Behaviour", fileName = "Behaviour")]
    public class BehaviourConfigSo : ScriptableObject
    {
        [Header("行为类型")]
        public BehaviourType type;
        [Header("行为控制类型")]
        public BehaviourControlType controlType;
        [Header("Body动画精灵路径")]
        public string bodySpritePath;
        [Header("皮肤动画精灵路径")]
        public string skinSpritePath;

        [Header("动画名称")]
        public string aniName;
        [Header("动画伸缩，1是原速度")]
        public float aniSpeedScale = 1;
        [Header("动画序列")]
        public List<AniFrame> aniSequence;
    }
    
    //动画位移配置
    [Serializable]
    public struct AniMove
    {
        //移动类型
        public BehaviourMoveType moveType;
        public float startSpeed;
    }

    //动画翻转配置
    [Serializable]
    public struct AniFlip
    {
        //翻转按键
        public List<KeyType> flipKeys;
    }

    //动画帧配置
    [Serializable]
    public struct AniFrame
    {
        [Header("动画帧精灵的文件名称")]
        public string aniFrameSprite;
        [Header("动画帧持续时间")]
        public float aniFrameDuration;
        [Header("动画帧音效")]
        public AudioClip aniFrameAudio;
        [Header("动画音效类型")]
        public AniFrameAudioType audioType;
    }

    //行为控制类型
    public enum BehaviourControlType
    {
        None,//无
        Once,//一次按键，播放到结束
        Sustain,//持续播放，一直到被打断
        SustainInverse,//持续按键才会继续播放
    }

    //行为移动控制类型
    public enum BehaviourMoveControlType
    {
        None,//无
        Sustain,//持续移动，无需按键控制
        SustainInverse,//持续按键才会移动
    }

    //行为移动类型
    public enum BehaviourMoveType
    {
        None,//无
        X,//X轴正向
        XInverse,//X轴反向
        Y,//Y轴正向
        YInverse,//Y轴反向
    }

    //动画帧音效类型
    public enum AniFrameAudioType
    {
        None,//无
        Once,//一次性
        Loop,//循环
    }
}