using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
    #region 帧切换
    //帧切换条件
    [Serializable]
    public class FrameSwitchCondition
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        [Header("切换类型")]
        public FrameSwitchType type;

        [Header("切换条件-时间")]
        public FrameSwitchTime time;
        [Header("切换条件-时间按键类型")]
        public FrameSwitchTimeOrKey timeOrKey;
        [Header("切换条件-时间按键无效类型")]
        public FrameSwitchTimeOrKeyInvalid timeOrKeyInvalid;
        [Header("切换条件-按键类型")]
        public FrameSwitchKey key;
        [Header("切换条件-按键无效类型")]
        public FrameSwitchKeyInvalid keyInvalid;
        [Header("切换条件-下落时距离地面百分比")]
        public FrameSwitchFallPercent fallPercent;
    }

    //切换条件-时间类型
    [Serializable]
    public class FrameSwitchTime
    {
        [Header("时间，毫秒")]
        public float time;
    }

    //切换条件-时间按键类型
    [Serializable]
    public class FrameSwitchTimeOrKey
    {
        [Header("时间，毫秒")]
        public float time;
        [Header("按键")]
        public List<KeyLimit> keys;
    }
    
    //切换条件-时间按键无效类型
    [Serializable]
    public class FrameSwitchTimeOrKeyInvalid
    {
        [Header("时间，毫秒")]
        public float time;
        [Header("按键")]
        public List<KeyLimit> keys;
    }
    
    //切换条件-按键类型
    [Serializable]
    public class FrameSwitchKey
    {
        [Header("按键")]
        public List<KeyLimit> keys;
    }

    //切换条件-按键无效类型
    [Serializable]
    public class FrameSwitchKeyInvalid
    {
        [Header("按键")]
        public List<KeyLimit> keys;
    }

    //切换条件-下落时距离地面百分比
    [Serializable]
    public class FrameSwitchFallPercent
    {
        [Header("百分比，1~100")]
        public float percent;
    }
    
    //帧切换条件枚举
    [Serializable]
    public enum FrameSwitchType
    {
        None,//无
        Time,//时间，到时间切换。行为、帧都可以。
        TimeOrKey,//时间-按键按下，到时间或按下切换。行为、帧都可以。
        TimeOrKeyInvalid,//时间-按键持续，到时间或不按切换。行为、帧都可以。
        Key,//按键，按下切换。行为、帧都可以。
        KeyInvalid,//按键，不按切换。行为、帧都可以。
        // ConditionHit,//条件-命中
        // ConditionLocalHorizontal,//条件-本地水平位置
        // ConditionLocalVertical,//条件-本地竖直位置
        // ConditionLocalVerticalPercent,//条件-本地竖直位置百分比
        FallPercent,//条件-下落时距离地面的距离百分比
        // ConditionHorizontal,//条件-世界水平位置
        // ConditionVertical,//条件-世界竖直位置
    }
    
    #endregion
    
    
    #region 行为切换

    //行为切换条件
    [Serializable]
    public class ActionSwitchCondition
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        [Header("切换类型")]
        public ActionSwitchType type;
        
        [Header("切换条件-按键无效类型")]
        public ActionSwitchKeyInvalid keyInvalid;
    }

    //切换条件-按键无效类型
    [Serializable]
    public class ActionSwitchKeyInvalid
    {
        [Header("按键")]
        public List<KeyLimit> keys;
    }
    
    //行为切换条件枚举
    [Serializable]
    public enum ActionSwitchType
    {
        None,//无
        // Time,//时间，到时间切换。行为、帧都可以。
        // TimeOrKey,//时间-按键按下，到时间或按下切换。行为、帧都可以。
        // TimeOrKeyInvalid,//时间-按键持续，到时间或不按切换。行为、帧都可以。
        // Key,//按键，按下切换。行为、帧都可以。
        KeyInvalid,//按键，不按切换。行为、帧都可以。
        // ConditionHit,//条件-命中
        // ConditionLocalHorizontal,//条件-本地水平位置
        // ConditionLocalVertical,//条件-本地竖直位置
        // ConditionLocalVerticalPercent,//条件-本地竖直位置百分比
        // FallPercent,//条件-下落时距离地面的距离百分比
        JumpToTop,//条件-跳跃时到达最高点
        FallToGround,//条件-下落至地面
        // ConditionHorizontal,//条件-世界水平位置
        // ConditionVertical,//条件-世界竖直位置
    }

    #endregion
    
    #region 联动切换

    //联动切换条件
    [Serializable]
    public class LinkageSwitchCondition
    {
        [Header("切换类型")]
        public LinkageSwitchType type;
        [Header("时间限制下有相同的行为")]
        public TimeLimitSame timeLimitSame;
    }

    //时间限制下有相同的行为
    [Serializable]
    public class TimeLimitSame
    {
        [Header("开始计时的帧")]
        public int startFrame;
        [Header("限时")]
        public float time;
    }

    //联动切换类型
    public enum LinkageSwitchType
    {
        None,//无
        Auto,//自动切换
        TimeControlLinear,//时间限制+控制下有相同的行为，那么当前行为结束后切换到下一个行为
    }
    
    #endregion
}