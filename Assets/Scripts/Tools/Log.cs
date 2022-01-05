using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //日志工具
    public static class Log
    {
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
        
        //普通异常日志-Debug
        public static void DebugError(string str)
        {
#if UNITY_EDITOR
            Debug.LogError(str);
#endif
        }

        //普通Info日志-Debug
        public static void DebugInfo(string str)
        {
#if UNITY_EDITOR
            Debug.Log(str);
#endif
        }
    }
}
