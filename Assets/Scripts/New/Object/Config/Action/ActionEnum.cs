using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using UnityEngine;

namespace New.Object.Config
{
    //行为或帧切换条件枚举
    [Serializable]
    public enum SwitchType
    {
        None,//无
        Key,//按键，按下切换。行为、帧都可以。
        KeyInvalid,//按键，不按切换。行为、帧都可以。
        Time,//时间，到时间切换。行为、帧都可以。
        TimeKey,//时间-按键按下，到时间或按下切换。行为、帧都可以。
        TimeKeyInvalid,//时间-按键持续，到时间或不按切换。行为、帧都可以。
        ConditionHit,//条件-命中
        ConditionLocalHorizontal,//条件-本地水平位置
        ConditionLocalVertical,//条件-本地竖直位置
        ConditionHorizontal,//条件-世界水平位置
        ConditionVertical,//条件-世界竖直位置
    }

    //切换操作行为
    [Serializable]
    public enum SwitchOptType
    {
        None,//无
        NextFrame,//下一帧
        BreakAction,//打断当前行为
        Linkage,//切换到联动行为
    }

    //打断类型，内部是自身、按键等改变；外部是受伤等等打断
    [Serializable]
    public enum BreakType
    {
        None,//无
        Inner,//内部
        Outer,//外部
    }

    //打断条件
    [Serializable]
    public enum BreakCondition
    {
        None,//无
        Any,//任意行为可打断，比如内部按键、外部伤害
        Weak,//弱打断，比如内部非强制技能、外部伤害攻击、外部弱控制等
        Strong,//强打断，比如内部强制技能、外部强控
        Force,//强制停止，比如死亡强制打断
    }
    
    //朝向更改类型
    [Serializable]
    public enum OrientationChangeType
    {
        None,//无
        Inner,//内部
        Outer,//外部
    }

    //朝向更改条件
    [Serializable]
    public enum OrientationChangeCondition
    {
        None,//无
        Auto,//自动修改
        Key,//按键更改
        Damage,//被攻击更改
        Control,//被控更改
    }

    //销毁类型
    [Serializable]
    public enum DestroyType
    {
        None,//无
        Inner,//内部
        Outer,//外部
    }

    //销毁条件
    [Serializable]
    public enum DestroyCondition
    {
        None,//无
        Auto,//自动
        ParentBreak,//父节点被打断
        Collider,//碰撞
        ColliderEnemy,//碰撞-敌方
    }

    //行为循环类型
    [Serializable]
    public enum ActionLoopType
    {
        None,//无
        Once,//一次
        Loop,//循环,无需控制
        LoopCommand,//循环，需要持续发出指令
    }

    //行为循环类型扩展
    public static class ActionLoopTypeExtend
    {
        //是否是循环类型
        public static bool IsLoop(ActionLoopType type)
        {
            return type == ActionLoopType.Loop || type == ActionLoopType.LoopCommand;
        }
    }

    //位移渐变类型
    [Serializable]
    public enum DisplacementFadeType
    {
        None,//无
        FadeBig,//渐强
        FadeSmall,//渐弱
        Constant,//匀速
        Blink,//瞬移到某个点
    }

    //位移朝向类型
    [Serializable]
    public enum DisplacementOrientationType
    {
        None,
        X,
        Y,
    }

    //位移控制类型
    [Serializable]
    public enum DisplacementControlType
    {
        None,
        Auto,
        KeyDown,//时间-按键按下
        KeySustain,//时间-按键持续
    }

    //位移结束类型
    [Serializable]
    public enum DisplacementEndType
    {
        None,//无
        Time,//时间
        Pos,//位置
        Distance,//距离
        Frame,//帧结束
        Velocity,//速度
    }
    
    //动画精灵类型
    [Serializable]
    public enum AniPartType
    {
        None,//无
        Body,//主身体
        SkinHair,//皮肤-头发
        SkinChest,//皮肤-胸
        SkinShoots,//皮肤-鞋
        SkinShoots2,//皮肤-鞋2
        SkinLegs,//皮肤-腿
        SkinLegs2,//皮肤-腿2
        SkinWeapon,//皮肤-武器
        SkinWeapon2,//皮肤-武器2
    }

    //动画精灵类型扩展
    public static class AniPartTypeExtend
    {
        //皮肤类型列表
        private static readonly List<AniPartType> _skinList;

        static AniPartTypeExtend()
        {
            _skinList = new List<AniPartType>();
            _skinList.Add(AniPartType.SkinHair);
            _skinList.Add(AniPartType.SkinChest);
            _skinList.Add(AniPartType.SkinShoots);
            _skinList.Add(AniPartType.SkinShoots2);
            _skinList.Add(AniPartType.SkinLegs);
            _skinList.Add(AniPartType.SkinLegs2);
            _skinList.Add(AniPartType.SkinWeapon);
            _skinList.Add(AniPartType.SkinWeapon2);
        }

        //获取所有皮肤类型列表
        public static List<AniPartType> GetAllSkin()
        {
            return _skinList;
        }
        
        //根据类型获取Hierarchy、Project中的名称
        public static string GetName(AniPartType type)
        {
            switch (type)
            {
                case AniPartType.None:
                    return null;
                case AniPartType.Body:
                    return ActionConstant.Body;
                case AniPartType.SkinHair:
                    return ActionConstant.SkinHair;
                case AniPartType.SkinChest:
                    return ActionConstant.SkinChest;
                case AniPartType.SkinShoots:
                    return ActionConstant.SkinShoots;
                case AniPartType.SkinShoots2:
                    return ActionConstant.SkinShoots2;
                case AniPartType.SkinLegs:
                    return ActionConstant.SkinLegs;
                case AniPartType.SkinLegs2:
                    return ActionConstant.SkinLegs2;
                case AniPartType.SkinWeapon:
                    return ActionConstant.SkinWeapon;
                case AniPartType.SkinWeapon2:
                    return ActionConstant.SkinWeapon2;
            }
            return null;
        }
    }

    //行为或帧切换条件枚举扩展类
    public static class SwitchTypeExtend
    {
        
        private static readonly List<SwitchType> TimeTypes;
        private static readonly List<SwitchType> ConditionTypes;

        static SwitchTypeExtend()
        {
            TimeTypes = new List<SwitchType>();
            ConditionTypes = new List<SwitchType>();
            
            TimeTypes.AddRange(new SwitchType[]
            {
                SwitchType.Time, SwitchType.TimeKey, SwitchType.TimeKeyInvalid
            });
            
            ConditionTypes.AddRange(new SwitchType[]
            {
                SwitchType.ConditionHit, 
                SwitchType.ConditionHorizontal, SwitchType.ConditionVertical, 
                SwitchType.ConditionLocalHorizontal, SwitchType.ConditionLocalVertical
            });
        }

        //是否是时间类型
        public static bool IsTimeType(SwitchType type)
        {
            return TimeTypes.Contains(type);
        }
        
        //是否是条件类型
        public static bool IsConditionType(SwitchType type)
        {
            return ConditionTypes.Contains(type);
        }
    }
}