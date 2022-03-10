using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
    //物理配置
    [Serializable]
    public class PhyCol
    {
        [Header("帧序列，序号为ActionFrame顺序")]
        public List<int> frameSequence;

        [Header("Box碰撞器配置")]
        public BoxCol box;
    }

    //Box碰撞器类型
    [Serializable]
    public class BoxCol
    {
        [Header("Box碰撞器帧配置")]
        public List<ColFrameConfig> frameConfig;

        [Header("是否是朝向敏感的")]
        public bool orientationSensitive;

        [Header("碰撞器位置类型")]
        public ColPos colPos;

        [Header("碰撞器判断类型")]
        public ColJudgeType colJudgeType;
    }

    //碰撞器帧配置
    [Serializable]
    public class ColFrameConfig
    {
        [Header("帧序列，序号为ActionFrame顺序")]
        public List<int> frameSequence;
        
        [Header("Box碰撞器中心偏移")]
        public Vector2 offset;
        
        [Header("Box碰撞器尺寸")]
        public Vector2 size;
    }

    // //碰撞器类型
    // public enum ColType
    // {
    //     None,
    //     NormalBox,//普通Box碰撞器
    // }
    
    //碰撞器位置
    public enum ColPos
    {
        None,
        XZ,//XZ轴碰撞器
        Y,//Y轴碰撞器
    }

    //碰撞器判断类型
    public enum ColJudgeType
    {
        None,
        Hit,//直接造成伤害
        Judge,//用来判断
        Body,//物体身体部分框
    }
}