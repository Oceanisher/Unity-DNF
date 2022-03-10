using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
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

    

    //位移朝向类型
    [Serializable]
    public enum DisplacementOrientationType
    {
        None,
        Forward,//当前朝向
        Backward,//当前朝向的逆向
        XForward,//X轴-正向
        XBackward,//X轴-反向
        YForward,//实际上是Local Y轴正向
        YBackward,//实际上是Local Y轴反向
        ZForward,//实际上是World Y正向
        ZBackward,//实际上是World Y反向
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
}