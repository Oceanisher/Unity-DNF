using System;
using System.Collections.Generic;
using Obj.Unit.Control.Item;
using UnityEngine;

namespace Obj.Unit.Control
{
    //战斗单元
    public class FightUnit : AbstractObjUnit
    {
        //Buff单元项
        private BuffUnitItem _buffUnitItem;
        //装备单元项
        private EquipmentUnitItem _equipmentUnitItem;

        //伤害信息
        private FightInnerInfo _info;
        
        //伤害碰撞事件发生
        public FightInfo OnColEnter(ColItemInfo info)
        {
            //TODO 已经存在或者处理次数超过0，那么暂不处理
            if (_info.HasAdded(info) || info.HandleCount > 0)
            {
                return null;
            }

            //TODO 暂时啥也不干，只返回伤害结果
            OnEnterPre(info, out var fightInfo);
            OnEnter(fightInfo);
            OnEnterPost(fightInfo);

            return fightInfo;
        }
        
        //伤害碰撞事件结束
        public FightInfo OnColExit(ColItemInfo info)
        {
            //TODO 如果处理次数超过0，那么暂不处理
            if (info.HandleCount > 0)
            {
                return null;
            }
            
            //TODO 暂时啥也不干
            OnExitPre(info, out var fightInfo);
            OnExit(fightInfo);
            OnExitPost(info, fightInfo);
            
            return fightInfo;
        }

        //碰撞清除
        public void OnColClear(List<ColItemInfo> infoList)
        {
            OnClear(infoList);
        }

        #region 战斗过程处理

        //碰撞进入前置处理
        private void OnEnterPre(ColItemInfo info, out FightInfo fightInfo)
        {
            _info.EnterPre(info, out fightInfo);
        }
        
        //碰撞进入处理
        private void OnEnter(FightInfo fightInfo)
        {
            //Buff处理
            _info.EnterBuff(fightInfo, null);
            //装备处理
            _info.EnterEquipment(fightInfo, null);
            //最终结果
            FightResult fightResult = FightResult.Build(
                FightResultType.Success, 
                FightDamageType.PD, 
                10f);
            _info.EnterResult(fightInfo, fightResult);
        }
        
        //碰撞进入后置处理
        private void OnEnterPost(FightInfo fightInfo)
        {
            
        }
        
        //碰撞离开前置处理
        private void OnExitPre(ColItemInfo info, out FightInfo fightInfo)
        {
            _info.FightInfoMap.TryGetValue(info, out fightInfo);
        }
        
        //碰撞离开处理
        private void OnExit(FightInfo fightInfo)
        {
            
        }

        //清除碰撞
        private void OnClear(List<ColItemInfo> infoList)
        {
            _info.ClearInfo(infoList);
        }
        
        //碰撞离开后置处理
        private void OnExitPost(ColItemInfo info, FightInfo fightInfo)
        {
            
        }

        #endregion
        
        #region AbstractObjUnit 接口

        public override void Init(ObjCore core)
        {
            base.Init(core);

            _buffUnitItem = new BuffUnitItem();
            _equipmentUnitItem = new EquipmentUnitItem();
            _info = FightInnerInfo.BuildInit();
            
            HasInit = true;
        }

        #endregion
    }

    #region 工具类

    //战斗内部信息
    public class FightInnerInfo
    {
        //战斗信息映射
        public Dictionary<ColItemInfo, FightInfo> FightInfoMap { get; private set; }

        public static FightInnerInfo BuildInit()
        {
            FightInnerInfo info = new FightInnerInfo();
            info.FightInfoMap = new Dictionary<ColItemInfo, FightInfo>();
            return info;
        }

        //是否已经存在了
        public bool HasAdded(ColItemInfo colItemInfo)
        {
            FightInfoMap.TryGetValue(colItemInfo, out var colIteInfo);
            return null != colIteInfo;
        }

        //碰撞进入前置处理
        public void EnterPre(ColItemInfo colItemInfo, out FightInfo fightInfo)
        {
            fightInfo = FightInfo.BuildInit();
            fightInfo.ColItemInfo = colItemInfo;
            FightInfoMap.Add(colItemInfo, fightInfo);
        }

        //碰撞进入-Buff处理
        public void EnterBuff(FightInfo fightInfo, BuffResult result)
        {
            fightInfo.BuffResult = result;
        }
        
        //碰撞进入-装备处理
        public void EnterEquipment(FightInfo fightInfo, EquipmentResult result)
        {
            fightInfo.EquipmentResult = result;
        }
        
        //碰撞进入-结果处理
        public void EnterResult(FightInfo fightInfo, FightResult result)
        {
            fightInfo.FightResult = result;
        }

        //清除信息
        public void ClearInfo(List<ColItemInfo> infoList)
        {
            foreach (var item in infoList)
            {
                FightInfoMap.Remove(item);
            }
        }
    }

    //战斗信息
    public class FightInfo
    {
        //碰撞信息
        public ColItemInfo ColItemInfo;
        //Buff处理结果
        public BuffResult BuffResult;
        //装备处理结果
        public EquipmentResult EquipmentResult;
        //最终结果
        public FightResult FightResult;

        public static FightInfo BuildInit()
        {
            FightInfo info = new FightInfo();
            return info;
        }
    }

    //最终战斗结果
    public class FightResult
    {
        //战斗结果
        public FightResultType ResultType;
        //伤害类型
        public FightDamageType DamageType;
        //最终伤害
        public float FinalDamage;

        public static FightResult Build(
            FightResultType resultType,
            FightDamageType damageType,
            float finalDamage)
        {
            FightResult result = new FightResult();
            result.ResultType = resultType;
            result.DamageType = damageType;
            result.FinalDamage = finalDamage;
            return result;
        }
    }

    //战斗结果枚举
    public enum FightResultType
    {
        None,
        Success,//成功造成伤害
        Dodge,//闪避
        Zero,//伤害归零
        Invincible,//无敌
        Error,//异常
    }

    //战斗结果枚举扩展
    public static class FightResultTypeExtend
    {
        //是否造成了伤害
        public static bool HasDamage(this FightResultType type)
        {
            return type == FightResultType.Success;
        }
    }

    //伤害类型
    public enum FightDamageType
    {
        None,
        PD,//物理伤害
        MD,//魔法伤害
        ID,//独立伤害
    }

    //伤害类型-扩展
    public static class FightDamageTypeExtend
    {
        public static string ShowName(this FightDamageType type)
        {
            switch (type)
            {
                case FightDamageType.PD:
                    return "物理伤害";
                case FightDamageType.MD:
                    return "魔法伤害";
                case FightDamageType.ID:
                    return "独立伤害";
                default:
                    return "未知";
            }
        }
    }

    #endregion
}