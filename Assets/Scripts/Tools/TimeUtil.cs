using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //Time工具
    public static class TimeUtil
    {

        #region Unity内部时间
        
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

        //获取Time.deltaTime时间，秒数
        public static float DeltaTime()
        {
            return Time.deltaTime;
        }

        #endregion

        #region 其他时间

        //获取当前时间戳，截止到秒数
        //字符串类型 ：20220414155004
        public static string GetTimestampStr()
        {
            DateTime now = DateTime.Now;
            return now.Year.ToString() + 
                   (now.Month > 9 ? now.Month.ToString() : "0" + now.Month.ToString()) +
                   (now.Day > 9 ? now.Day.ToString() : "0" + now.Day.ToString()) +
                   (now.Hour > 9 ? now.Hour.ToString() : "0" + now.Hour.ToString()) +
                   (now.Minute > 9 ? now.Minute.ToString() : "0" + now.Minute.ToString()) +
                   (now.Second > 9 ? now.Second.ToString() : "0" + now.Second.ToString())
                ;
        }

        #endregion
    }
}