using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    //按键信息，主要附加一些额外的信息
    public struct KeyInfo
    {
        //按键
        public KeyCode KeyCode;
        //按键类型
        public KeyType KeyType;
        //行为类型
        public BehaviourType BehaviourType;
        //是否强制
        public bool IsForce;
        //是否是持续按键
        public bool IsSustain;
        //按键时间，毫秒
        public float Time;
    }
}