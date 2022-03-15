using System;
using System.Collections;
using System.Collections.Generic;
using Obj.Config.Action.Structure;
using UnityEngine;

namespace Obj.Config.Action
{
    //行为配置
    [CreateAssetMenu(menuName = "Config/Obj/Action", fileName = "ActionSo")]
    [Serializable]
    public class ActionSo : ScriptableObject
    {
        [Header("行为名称")]
        public string actionName;
        [Header("行为展示名称")]
        public string actionShowName;
        [Header("行为唯一标识")]
        public ActionType type;
        [Header("行为唯一子级标识")]
        public ActionSubType subType;
        [Header("行为唯一ID")]
        public string uuid;
        [Header("循环类型")]
        public ActionLoopType loopType;
        
        [Header("帧配置")]
        public ActionFrame actionFrame;
        [Header("物理配置")]
        public ActionPhysics actionPhysics;
        [Header("位移配置")]
        public ActionDisplacement actionDisplacement;
        [Header("行为其他配置")]
        public ActionOther actionOther;
    }
}