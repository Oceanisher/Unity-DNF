using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //日志工具
    public class LogUtil
    {
        private LogUtil() { }

        //普通异常日志
        public static void Error(string str)
        {
            Debug.LogError(str);
        }

        //普通Info日志
        public static void Info(string str)
        {
            Debug.Log(str);
        }
    }
}
