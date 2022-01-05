using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace New.Object.Config
{
    //行为配置
    [CreateAssetMenu(menuName = "Config/Obj/Action", fileName = "ActionSo")]
    [Serializable]
    public class ActionSo : ScriptableObject
    {
        [Header("行为名称")]
        public string actionName;
        [Header("循环类型")]
        public ActionLoopType loopType;
        
        //TODO 强制类型应该写到用户技能选择配置中，暂时先配置到这里
        [Header("初始帧打断限制，也就是说本行为本身是强制技能还是普通技能等")]
        public BreakCondition changeLimit;
        [Header("物理配置")]
        public ActionPhysics actionPhysics;
        [Header("位移配置")]
        public ActionDisplacement actionDisplacement;
        [Header("帧配置")]
        public ActionFrame actionFrame;
        
        [Header("是否有联动")]
        public bool hasLinkage;
        [Header("联动配置")]
        public Linkage linkage;
    }
}