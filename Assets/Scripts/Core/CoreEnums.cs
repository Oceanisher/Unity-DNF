using System;
using System.Collections.Generic;

namespace Core
{
    //场景类型
    [Obsolete]
    public enum SceneType
    {
        None,//无
        Town,//城镇场景
        Fight,//战斗场景
    }
    
    //物体类型
    public enum ObjType
    {
        None,//无
        Character,//角色类
        Skill,//技能
    }
    
    //角色类型
    public enum CharacterType
    {
        None,//无
        Player,//玩家
        Enemy,//敌人
        Neutral,//中立生物
    }

    //朝向
    public enum OrientationType
    {
        Left,//左
        Right,//右
    }

    //碰撞体类型
    public enum ColliderType
    {
       None,//无
       Damage,//伤害
       Judge,//碰撞判断
       Entity,//实体，代表每个物体的实际碰撞
    }
    
    //行为类型
    public enum BehaviourType
    {
        None,//无
        Common,//通用，技能类等常用
        Idle,//空闲
        StandTown,//站立-城镇
        StandFight,//站立-备战
        Walk,//走
        Run,//跑
        Hit,//普攻
        Jump,//跳
        Levitate,//上挑
        SkillFirst,//技能1
    }

    //按键类型
    public enum KeyType
    {
        None,//无
        Up,//上
        Down,//下
        Left,//左
        Right,//右
        Hit,//普攻
        Jump,//跳
        Levitate,//上挑
        SkillFirst,//技能1
    }

    //按键类型与行为映射
    public class KeyBehaviourMap
    {
        private static Dictionary<KeyType, BehaviourType> _map = new Dictionary<KeyType, BehaviourType>();

        private KeyBehaviourMap(){}
        
        static KeyBehaviourMap()
        {
            _map.Add(KeyType.Up, BehaviourType.Walk);
            _map.Add(KeyType.Down, BehaviourType.Walk);
            _map.Add(KeyType.Left, BehaviourType.Walk);
            _map.Add(KeyType.Right, BehaviourType.Walk);
            _map.Add(KeyType.Hit, BehaviourType.Hit);
            _map.Add(KeyType.Jump, BehaviourType.Jump);
            _map.Add(KeyType.Levitate, BehaviourType.Levitate);
            _map.Add(KeyType.SkillFirst, BehaviourType.SkillFirst);
        }

        //根据KeyType获取BehaviourType
        public static BehaviourType Convert(KeyType type)
        {
            _map.TryGetValue(type, out var behaviourType);
            return behaviourType;
        }
    }
}
