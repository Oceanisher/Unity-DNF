using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //Time工具
    public class TimeUtil
    {
        private TimeUtil() {}

        //获取FixedUpdate()间隔时间，毫秒
        public static float FixedDeltaTimeMs()
        {
            return Time.fixedDeltaTime * 1000;
        }

        //获取Time.time时间，毫秒数
        public static float TimeMs()
        {
            return Time.time * 1000;
        }

        //获取Time.deltaTime时间，毫秒数
        public static float DeltaTimeMs()
        {
            return Time.deltaTime * 1000;
        }
    }
}