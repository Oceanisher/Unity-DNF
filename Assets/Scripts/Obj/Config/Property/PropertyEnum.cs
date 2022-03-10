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
    public enum ObjPosState
    {
        None,//无
        Ground,//地面-正常
        GroundControlled,//地面-被控中
        GroundHurt,//地面-受伤中
        GroundSkill,//地面-技能中
        GroundLie,//地面-倒地中
        GroundGetUp,//地面-起身中
        Sky,//空中-正常
        SkyControlled,//空中-被控中
        SkyHurt,//空中-受伤中
        SkySkill,//空中-技能中
        SkyLie,//空中-倒地中
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
            GroundList.Add(ObjPosState.GroundSkill);
            GroundList.Add(ObjPosState.GroundGetUp);
            SkyList.Add(ObjPosState.Sky);
            SkyList.Add(ObjPosState.SkyControlled);
            SkyList.Add(ObjPosState.SkyHurt);
            SkyList.Add(ObjPosState.SkyLie);
            SkyList.Add(ObjPosState.SkySkill);
        }
        
        //是否在地面
        public static bool IsGround(ObjPosState state)
        {
            return GroundList.Contains(state);
        }

        //是否在空中
        public static bool IsSKy(ObjPosState state)
        {
            return SkyList.Contains(state);
        }
        
        //是否受伤
        public static bool IsHurt(ObjPosState state)
        {
            return state == ObjPosState.GroundHurt || state == ObjPosState.SkyHurt;
        }
        
        //是否受控
        public static bool IsControlled(ObjPosState state)
        {
            return state == ObjPosState.GroundControlled || state == ObjPosState.SkyControlled;
        }
        
        //是否技能释放中
        public static bool IsSkill(ObjPosState state)
        {
            return state == ObjPosState.GroundSkill || state == ObjPosState.SkySkill;
        }
    }
}