using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Obj.Config.Property
{
    //物体属性配置
    public class ObjProperties
    {
        //固有属性
        public InherentProperties InherentProperties;
        //动态属性
        public DynamicProperties DynamicProperties;
        //特有属性
        public SpecialProperties SpecialProperties;
    }
    
    //物体固有属性
    public class InherentProperties
    {
        //物体类型
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjType ObjType;
        //生命周期类型
        [JsonConverter(typeof(StringEnumConverter))]
        public LifeCycleType LifeCycleType;
        //是否有父节点
        public bool HasParent;
        //是否接受按键输入
        public bool AllowKeyInput;
        //角色类固有属性
        public CharacterInherentProperties CharacterInherentProperties;
    }
    
    //物体动态属性
    public class DynamicProperties
    {
        //向上轴高度
        public float UpAxisHeight;

        //角色类动态属性
        public CharacterDynamicProperties CharacterProperties;
    }

    //物体独有属性
    public class SpecialProperties
    {
        //角色固有属性
        public CharacterSpecialProperties CharacterSpecialProperties;
    }

    //角色固有属性
    public class CharacterInherentProperties
    {
        //飞行类型
        [JsonConverter(typeof(StringEnumConverter))]
        public FlyType FlyType;
    }

    //角色类动态属性
    public class CharacterDynamicProperties
    {
        //朝向
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation;
        //位置状态
        [JsonConverter(typeof(StringEnumConverter))]
        public ObjPosState PosState;
        //允许倒地
        public bool AllowLie;
        //允许坠落
        public bool AllowFall;
        //是否在战斗中
        public bool IsInFight;
    }

    //角色类独有属性
    public class CharacterSpecialProperties
    {
        //力
        public float Strength;
        //智
        public float Agility;
        //敏
        public float Intelligence;
        //血
        public float HP;
        //蓝
        public float MP;
        //物理攻击 physical attack
        public float PA;
        //魔法攻击 magic attack
        public float MA;
        //物理抗性 physical resistance
        public float PR;
        //魔法抗性 magic resistance
        public float MR;
        //攻击速度 attack speed
        public float AS;
        //释放速度 release speed
        public float RS;
        //命中率 hit rate
        public float HR;
        //回避率 avoidance rate
        public float AR;
        //僵直，对敌方伤害的僵直加成
        public float Rigidity;
        //硬直，对僵直的抗性
        public float Stiffness;

        //行走速度
        public Vector3 WalkSpeed;
        //跑步速度
        public Vector3 RunSpeed;
        
        //冰属性强化
        public float IceIntensify;
        //冰属性抗性
        public float IceResistance;
        //火属性强化
        public float FireIntensify;
        //火属性抗性
        public float FireResistance;
        //光属性强化
        public float LightIntensify;
        //光属性抗性
        public float LightResistance;
        //暗属性强化
        public float DarkIntensify;
        //暗属性抗性
        public float DarkResistance;
    }
}