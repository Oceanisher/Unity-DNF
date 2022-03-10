using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //日志工具
    public static class Log
    {
        //普通异常日志
        public static void Error(string str, LogModule module)
        {
            Debug.LogError(str);
        }

        //普通Info日志
        public static void Info(string str, LogModule module)
        {
            Debug.Log(str);
        }
        
        //普通异常日志-Debug
        public static void DebugError(string str, LogModule module)
        {
#if UNITY_EDITOR
            Debug.LogError(str);
#endif
        }

        //普通Info日志-Debug
        public static void DebugInfo(string str, LogModule module)
        {
#if UNITY_EDITOR
            Debug.Log(str);
#endif
        }
    }

    //日志级别 TODO 未来配置用到
    public enum LogLevel
    {
        Error,//异常日志
        Warning,//告警日志
        Info,//信息日志
        All,//所有
    }

    //日志模块
    public enum LogModule
    {
        None,
        Manager,
        ObjCore,
        Tool,
    }
}
