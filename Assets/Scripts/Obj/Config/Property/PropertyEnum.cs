using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obj.Config.Property
{
    //物体类型
    public enum ObjType
    {
        None,//无
        Character,//角色
        Active,//活跃物体
        Effect,//特效
    }

    //生命周期类型
    public enum LifeCycleType
    {
        None,//无
        Permanent,//永久
        Time,//时间
        Condition,//条件
    }

    //角色飞行类型
    public enum FlyType
    {
        None,//无
        Ground,//陆行
        Fly,//飞行
        GroundFly,//陆空混合
    }

    //物体朝向
    public enum Orientation
    {
        None,//无
        Left,//左
        Right,//右
    }

    //角色位置状态
    //带有Ground、Sky的都是明确所在位置的，其他的需要借助Displacement来判断
    public enum ObjPosState
    {
        None,//无
        
        Ground,//地面-正常，静止+走+跑
        GroundControlled,//地面-被控中
        GroundHurt,//地面-受伤中
        GroundLie,//地面-倒地中
        GroundGetUp,//地面-起身中
        
        Sky,//空中-正常，跳+下落
        SkyControlled,//空中-被控中
        SkyHurt,//空中-受伤中
        
        Jump,//跳起
        Rise,//上升
        Drop,//下落
        Fall,//坠落

        Skill,//技能中
    }

    //角色位置状态扩展
    public static class ObjPosStateExtend
    {
        private static readonly List<ObjPosState> GroundList = new List<ObjPosState>();
        private static readonly List<ObjPosState> SkyList = new List<ObjPosState>();

        static ObjPosStateExtend()
        {
            GroundList.Add(ObjPosState.Ground);
            GroundList.Add(ObjPosState.GroundControlled);
            GroundList.Add(ObjPosState.GroundHurt);
            GroundList.Add(ObjPosState.GroundLie);
            GroundList.Add(ObjPosState.GroundGetUp);
            SkyList.Add(ObjPosState.Sky);
            SkyList.Add(ObjPosState.SkyControlled);
            SkyList.Add(ObjPosState.SkyHurt);
        }
        
        //是否在地面
        public static bool IsGround(this ObjPosState state)
        {
            return GroundList.Contains(state);
        }

        //是否在空中
        public static bool IsSKy(this ObjPosState state)
        {
            return SkyList.Contains(state);
        }
        //
        // //是否是处于不能释放技能状态
        // //TODO 未来例如替身草人等，可以在受伤等状态下释放
        // public static bool IsCantSkillState(this ObjPosState state)
        // {
        //     return IsHurt(state) || IsControlled(state)
        //                          || ObjPosState.GroundLie == state
        //                          || ObjPosState.GroundGetUp == state;
        // }
        //
        
        //是否受伤
        public static bool IsHurt(this ObjPosState state)
        {
            return state == ObjPosState.GroundHurt || state == ObjPosState.SkyHurt;
        }
        
        //是否受控
        public static bool IsControlled(this ObjPosState state)
        {
            return state == ObjPosState.GroundControlled || state == ObjPosState.SkyControlled;
        }
    }
}