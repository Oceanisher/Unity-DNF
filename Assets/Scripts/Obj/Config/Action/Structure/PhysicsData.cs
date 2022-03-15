using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
    //物理配置
    [Serializable]
    public class Phy
    {
        [Header("帧序列，序号为ActionFrame顺序")]
        public List<int> frameSequence;
        
        [Header("碰撞器判断类型")]
        public PhyJudgeType judgeType;

        [Header("伤害配置")]
        public PhyDamage damage;

        [Header("碰撞器配置")]
        public List<PhyCol> colList;
    }

    #region 伤害配置

    //物理伤害配置
    [Serializable]
    public class PhyDamage
    {
        [Header("伤害类型")]
        public PhyDamageType type;

        [Header("一次性伤害")]
        public PhyDamageOnce damageOnce;
    }

    //物理伤害类型
    [Serializable]
    public enum PhyDamageType
    {
        None,
        Once,//一次性伤害，一个技能实例期间该物理碰撞器只生效一次
    }

    //一次性伤害
    [Serializable]
    public class PhyDamageOnce
    {
        [Header("伤害值")]
        public float damage;
    }

    #endregion

    #region 碰撞器配置

    //Box碰撞器类型
    [Serializable]
    public class PhyCol
    {
        [Header("Box碰撞器帧配置")]
        public List<ColFrameConfig> frameConfig;

        [Header("是否是朝向敏感的")]
        public bool orientationSensitive;

        [Header("碰撞器位置类型")]
        public ColPos colPos;
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

    //碰撞器位置
    public enum ColPos
    {
        None,
        XZ,//XZ轴碰撞器
        Y,//Y轴碰撞器
    }

    //碰撞器判断类型
    public enum PhyJudgeType
    {
        None,
        Hit,//直接造成伤害
        Judge,//用来判断
        Body,//物体身体部分框
    }

    #endregion
}