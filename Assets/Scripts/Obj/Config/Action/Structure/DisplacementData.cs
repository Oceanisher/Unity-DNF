using System;
using System.Collections.Generic;
using Sys.Config;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
    //位移配置
    [Serializable]
    public class Displacement
    {
        [Header("帧序列，序号为ActionFrame顺序")]
        public List<int> frameSequence;
        [Header("位移类型")]
        public DisplacementType type;

        [Header("位移-匀速XZ轴移动")]
        public DisplacementMove move;
        [Header("位移-匀速&按键按住XZ移动")]
        public DisplacementMoveKeyHold moveKeyHold;
        [Header("位移-跳跃")]
        public DisplacementJump jump;
    }
    

    //位移配置-匀速XZ移动
    //只要帧在播放，那么就持续匀速位移
    [Serializable]
    public class DisplacementMove
    {
        [Header("位移速度类型")]
        public DisplacementVelocityType velocityType;
        [Header("自定义位移速度-当位移速度类型为自定义时")]
        public Vector3 customVelocity;
        [Header("朝向类型")]
        public DisplacementOrientationType orientationType;
    }
    
    //位移配置-匀速&按键按住XZ移动
    //帧+按键同时满足，才能持续位移
    [Serializable]
    public class DisplacementMoveKeyHold : DisplacementMove
    {
        [Header("位移控制类型-按键，对应的按键类型，可以是多个")]
        public List<KeyLimit> keys;
    }

    //位移配置-跳起
    [Serializable]
    public class DisplacementJump
    {
        [Header("初始速度，游戏米/秒")]
        public float startVelocity;
    }
    
    // //位移配置-下落/坠落
    // [Serializable]
    // public class DisplacementFallOrDrop
    // {
    //     [Header("加速度")]
    //     public float accelerate;
    // }
    
    //按键限制条件
    [Serializable]
    public class KeyLimit
    {
        [Header("按键")]
        public KeyType key;
        
        [Header("按键状态")]
        public List<KeyState> keyStateList;
    }
    
    #region 枚举
    
    //位移渐变类型
    [Serializable]
    public enum DisplacementType
    {
        None,//无
        //FadeBig,//渐强，暂无
        //FadeSmall,//渐弱，暂无
        Move,//移动，XZ轴匀速移动
        MoveKeyHold,//按键移动，XZ轴匀速移动
        Jump,//跳起类型：给予一个初速度、以及反向的加速度
        // Drop,//下落类型：从顶点下落、以及同向的加速度
        // Fall,//坠落类型：从顶点下落、以及同向的加速度
        // SkyStay,//滞空类型
        //Blink,//瞬移到某个点
    }
    
    //位移速度类型
    [Serializable]
    public enum DisplacementVelocityType
    {
        None,
        CharacterWalk,//使用角色走动速度
        CharacterRun,//使用角色跑动速度
        Custom,//自定义速度
    }
    
    #endregion
}