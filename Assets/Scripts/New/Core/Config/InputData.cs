using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace New.Core.Config
{
    //输入类型枚举
    public enum KeyType
    {
        None,//无
        Any,//任意按键
        Left,//左
        Right,//右
        Up,//上
        Down,//下
        Hit,//攻击
        Jump,//跳跃
        Levitate,//上挑
        Skill1,//技能1
        Skill2,//技能2
        Skill3,//技能3
        Skill4,//技能4
        Skill5,//技能5
        Skill6,//技能6
        Skill7,//技能7
        Skill8,//技能8
        Skill9,//技能9
    }

    //按键状态
    public enum KeyState
    {
        None,//无
        Single,//单击
        SingleHold,//单击按住
        SingleHoldRelease, //单击按住松开
        
        Double,//双击
        DoubleHold,//双击按住
        DoubleHoldRelease,//双击按住松开
    }

    //按键状态扩展
    public static class KeyStateExtend
    {
        //是否是Hold状态
        public static bool IsHold(KeyState keyState)
        {
            return keyState == KeyState.SingleHold || keyState == KeyState.DoubleHold;
        }

        //是否是HoldRelease状态
        public static bool IsHoldRelease(KeyState keyState)
        {
            return keyState == KeyState.SingleHoldRelease || keyState == KeyState.DoubleHoldRelease;
        }
    }
}