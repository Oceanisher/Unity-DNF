using System;
using System.Collections;
using System.Collections.Generic;
using Obj.Config.Property;
using Tools;
using UnityEngine;

namespace Obj.Config.Action.Structure
{
    //行为帧配置
    [Serializable]
    public class ActionFrame
    {
        // [Header("是否可顿帧")]
        // public bool canFreeze;
        [Header("帧动画信息")]
        public List<AniFrameInfo> aniFrameInfos;
        [Header("顿帧信息，攻击主动顿帧")]
        public FrameFreezeInfo frameFreezeInfo;
        [Header("帧切换条件：切换到下一帧的条件")]
        public List<FrameSwitchCondition> frameSwitchConditions;
        // [Header("打断条件")]
        // public List<BreakLimit> breakLimits;
        [Header("朝向更改条件")]
        public List<OrientationSwitchCondition> orientationLimits;
        // [Header("销毁")]
        // public List<DestroyLimit> destroyLimits;

        //当前帧是否可打断
        // public bool CanBreak(int index, BreakType checkType, BreakCondition checkCondition)
        // {
        //     if (CollectionUtil.IsEmpty(breakLimits))
        //     {
        //         return false;
        //     }
        //     
        //     //只要有一个可打断就可以
        //     foreach (var item in breakLimits)
        //     {
        //         if (item.CanBreak(checkType, checkCondition))
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }
    }

    //全局图形配置
    [Serializable]
    public class GlobalGraphics
    {
        //文件路径连接符
        private static readonly string PathLink = "/";
        
        [Header("正向朝向")]
        public Orientation positiveOrientation;
        [Header("每单位像素数量，默认100")]
        public int ppu;
        [Header("精灵数量，Body与皮肤必须数量一致。偏移文件中数据的数量也必须一致。文件名命名从0开始，以数字为文件名。")]
        public int spriteCount;
        [Header("是否使用皮肤")]
        public bool hasSkin;
        [Header("Body精灵路径，Resources目录下相对路径，一直到Body/Default结束，必填")]
        public string bodyPath;
        [Header("Skin精灵路径，Resources目录下相对路径，一直到Skin/Default结束")]
        public string skinPath;
        [Header("使用的皮肤列表")]
        public List<AniPartType> skinUseList;

        [Header("精灵统一偏移量")]
        public Vector2Int offset;
        [Header("Body偏移文件")]
        public TextAsset bodyOffsetTxt;
        [Header("Hair偏移文件")]
        public TextAsset hairOffsetTxt;
        [Header("Chest偏移文件")]
        public TextAsset chestOffsetTxt;
        [Header("Shoots偏移文件")]
        public TextAsset shootsOffsetTxt;
        [Header("Shoots2偏移文件")]
        public TextAsset shoots2OffsetTxt;
        [Header("Legs偏移文件")]
        public TextAsset legsOffsetTxt;
        [Header("Legs2偏移文件")]
        public TextAsset legs2OffsetTxt;
        [Header("Weapon偏移文件")]
        public TextAsset weaponOffsetTxt;
        [Header("Weapon2偏移文件")]
        public TextAsset weapon2OffsetTxt;

        //获取精灵
        public List<Sprite> GetSprites(AniPartType type)
        {
            string path = "";
            switch (type)
            {
                case AniPartType.None:
                    path = "";
                    break;
                case AniPartType.Body:
                    path = bodyPath + PathLink;
                    break;
                default:
                    path = skinPath + PathLink + AniPartTypeExtend.GetName(type) + PathLink;
                    break;
            }

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            List<Sprite> list = new List<Sprite>();

            for (int i = 0; i < spriteCount; i++)
            {
                Sprite sprite = ResourceUtil.GetSprite(path + i);
                list.Add(sprite);
            }

            return list;
        }

        //获取精灵偏移
        public List<Vector2Int> GetSpriteOffset(AniPartType type)
        {
            TextAsset textAsset = null;
            switch (type)
            {
                case AniPartType.Body:
                    textAsset = bodyOffsetTxt;
                    break;
                case AniPartType.SkinHair:
                    textAsset = hairOffsetTxt;
                    break;
                case AniPartType.SkinChest:
                    textAsset = chestOffsetTxt;
                    break;
                case AniPartType.SkinShoots:
                    textAsset = shootsOffsetTxt;
                    break;
                case AniPartType.SkinShoots2:
                    textAsset = shoots2OffsetTxt;
                    break;
                case AniPartType.SkinLegs:
                    textAsset = legsOffsetTxt;
                    break;
                case AniPartType.SkinLegs2:
                    textAsset = legs2OffsetTxt;
                    break;
                case AniPartType.SkinWeapon:
                    textAsset = weaponOffsetTxt;
                    break;
                case AniPartType.SkinWeapon2:
                    textAsset = weapon2OffsetTxt;
                    break;
            }

            if (null == textAsset)
            {
                return null;
            }

            return GetSpriteOffset(textAsset);
        }

        //文件资源读取
        private List<Vector2Int> GetSpriteOffset(TextAsset textAsset)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            string [] lineArray = textAsset.text.Split('\n');
            foreach (var str in lineArray)
            {
                string [] vStr = str.Split(' ');
                Vector2Int v = new Vector2Int(int.Parse(vStr[0]), int.Parse(vStr[1]));
                list.Add(v);
            }

            return list;
        }
    }

    //行为物理配置
    [Serializable]
    public class ActionPhysics
    {
        [Header("物理配置")]
        public List<Phy> infos;
    }
    
    //行为位移配置
    [Serializable]
    public class ActionDisplacement
    {
        [Header("位移列表")]
        public List<Displacement> infos;
    }

    //行为其他配置
    [Serializable]
    public class ActionOther
    {
        [Header("行为结束配置：满足条件后该行为结束")]
        public List<ActionSwitchCondition> actionStopCondition;
        [Header("联动配置")]
        public Linkage linkage;
    }

    //帧动画信息
    [Serializable]
    public class AniFrameInfo
    {
        [Header("精灵序列")]
        public int spriteSequence;
        [Header("动画帧音效")]
        public AudioClip aniFrameAudio;
    }

    //顿帧信息
    [Serializable]
    public class FrameFreezeInfo
    {
        [Header("帧序列")]
        public List<int> frameIndexes;

        [Header("顿帧时间，MS")]
        public float time;
    }

    //打断条件
    [Serializable]
    public class BreakLimit
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        [Header("打断类型")]
        public BreakType type;
        [Header("打断条件")]
        public BreakCondition condition;

        //是否可打断
        public bool CanBreak(BreakType checkType, BreakCondition checkCondition)
        {
            //任意None导致不可打断
            if (BreakType.None == type
                || BreakType.None == checkType
                || BreakCondition.None == condition
                || BreakCondition.None == checkCondition)
            {
                return false;
            }
            
            //同为内部或外部才行
            if (type != checkType)
            {
                return false;
            }

            return (int)checkType >= (int)condition;
        }
    }

    #region 朝向更改配置
    
    //朝向更改条件
    [Serializable]
    public class OrientationSwitchCondition
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        [Header("朝向更改条件")]
        public OrientationSwitchType type;
        [Header("朝向更改条件-按键类型")]
        public OrientationSwitchKey key;
    }

    //朝向更改-按键
    [Serializable]
    public class OrientationSwitchKey
    {
        [Header("朝向更改按键限制")]
        public List<KeyLimit> keyLimits;
        [Header("变更后的朝向")]
        public Orientation orientation;
    }
    
    //朝向更改条件
    [Serializable]
    public enum OrientationSwitchType
    {
        None,//无
        Key,//按键更改
        // Auto,//自动修改
        // Damage,//被攻击更改
        // Control,//被控更改
    }
    
    #endregion

    //销毁类型
    [Serializable]
    public class DestroyLimit
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        [Header("销毁类型")]
        public DestroyType type;
        [Header("销毁条件")]
        public DestroyCondition condition;
    }

    //行为或帧切换条件
    [Serializable]
    public class SwitchLimit
    {
        [Header("帧序列")]
        public List<int> frameIndexes;
        
        [Header("切换类型")]
        public FrameSwitchType switchType;

        [Header("切换操作类型")]
        public SwitchOptType switchOptType;
        
        [Header("时间类型-时间（MS）")]
        public float time;

        [Header("按键")]
        public List<KeyLimit> timeKeys;
        
        [Header("条件类型-限制数值（GM游戏米）")]
        public float conditionLimitMeter;

        [Header("条件类型-限制数值（速度）")]
        public float conditionLimitVelocity;

        [Header("条件类型-限制百分比，0~100")]
        public int conditionPercent;
    }

    //联动信息
    [Serializable]
    public class Linkage
    {
        [Header("切换条件")]
        public LinkageSwitchCondition condition;
        [Header("联动行为")]
        public ActionSo linkageAction;
    }

    //图形精灵信息
    [Serializable]
    public class GraphicsSpriteInfo
    {
        [Header("精灵文件名称，不带后缀")]
        public string fileName;
        [Header("精灵文件名称")]
        public Vector2Int spriteOffset;
    }

    //行为唯一标识
    public enum ActionType
    {
        None,
        CharacterDefaultTown,//角色类-城镇默认
        CharacterDefaultFight,//角色类-战斗默认
        CharacterWalk,//角色类-走
        CharacterRun,//角色类-跑
        CharacterJump,//角色类-跳
        CharacterDrop,//角色类-降落
        CharacterFall,//角色类-坠落
        CharacterHit,//角色类-普攻
    }

    //行为唯一子级标识
    public enum ActionSubType
    {
        None,
        CharacterHit_1,//角色类-普攻1
        CharacterHit_2,//角色类-普攻2
        CharacterHit_3,//角色类-普攻3
    }

    //行为唯一标识扩展
    // public static class ActionTypeExtend
    // {
    //     //弱行为-可被其他技能等打断
    //     public static bool IsWeak(this ActionType o)
    //     {
    //         return o == ActionType.CharacterDefaultTown
    //                || o == ActionType.CharacterDefaultFight
    //                || o == ActionType.CharacterWalk
    //                || o == ActionType.CharacterRun;
    //     }
    // }
}