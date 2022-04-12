using System;
using System.Collections.Generic;
using System.Collections;
using App.Config;
using Tools;
using UnityEditor;
using UnityEngine;

namespace App.Editor.Build
{
    //自动化构建工具
    public static class DefaultBuilder
    {
        //构建Mac版本-开发版本
        public static void BuildMacDevelop()
        {
            //获取参数
            Dictionary<CommandLine, string> commandMap = GetArgs();
            //获取输出路径
            commandMap.TryGetValue(CommandLine.CustomOutputPath, out var outPath);
            //获取配置文件
            AppConfigSo config = ResourceUtil.GetAppConfig(BuildTarget.StandaloneOSX);

            //配置版本
            PlayerSettings.bundleVersion = config.appVersion;
            PlayerSettings.productName = config.name;
            
            BuildPipeline.BuildPlayer(
                GetBuildScenes(),
                outPath,
                BuildTarget.StandaloneOSX,
                BuildOptions.Development
                );
        }
        
        //从构建指令中提取参数
        private static Dictionary<CommandLine, string> GetArgs()
        {
            Dictionary<CommandLine, string> argMap = new Dictionary<CommandLine, string>();

            bool isDouble = false;
            CommandLine doubleEnum = CommandLine.None;
            bool isSingle = false;
            CommandLine singleEnum = CommandLine.None;
            
            //获取指令中的所有参数
            string[] args = System.Environment.GetCommandLineArgs();
            Debug.Log("[DefaultBuilder]CommandLine start.");
            foreach (var str in args)
            {
                Debug.Log($"{str}");
            }
            Debug.Log("[DefaultBuilder]CommandLine end.");
            foreach (var arg in args)
            {
                //如果是 "--" 指令
                if (isDouble)
                {
                    //"--"指令后带等号
                    int splitIndex = arg.IndexOf("=");
                    argMap.Add(doubleEnum, arg.Substring(splitIndex + 1));
                    
                    isDouble = false;
                    doubleEnum = CommandLine.None;
                    continue;
                }
                //如果是 "-" 指令
                if (isSingle)
                {
                    argMap.Add(singleEnum, arg);

                    isSingle = false;
                    singleEnum = CommandLine.None;
                    continue;
                }
                
                //非命令，跳过
                if (!CommandLineEnumMap.CommandMap.ContainsKey(arg))
                {
                    continue;
                }
                
                //如果是 "--" 指令
                if (arg.StartsWith("--"))
                {
                    isDouble = true;
                    CommandLineEnumMap.CommandMap.TryGetValue(arg, out doubleEnum);
                    continue;
                }
                //如果是 "-" 指令
                if (arg.StartsWith("-"))
                {
                    isSingle = true;
                    CommandLineEnumMap.CommandMap.TryGetValue(arg, out singleEnum);
                    continue;
                }
            }

            return argMap;
        }

        //获取需要打包的Scene
        //从EditorBuildSettings中获取
        private static string[] GetBuildScenes()
        {
            List<string> sceneNames = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                {
                    continue;
                }

                if (e.enabled)
                {
                    sceneNames.Add(e.path);
                }
            }

            return sceneNames.ToArray();
        }

        //命令行枚举，目前只列举需要使用的
        public enum CommandLine
        {
            None,
            /*******平台参数*******/
            BuildTarget,//构建平台
            
            /*******自定义参数*******/
            CustomOutputPath,//输出路径
        }

        //命令行与枚举映射
        public static class CommandLineEnumMap
        {
            public static string BuildTarget = "-buildTarget";
            public static string CustomOutputPath = "--outPath";

            public static List<string> StrList = new List<string>();
            public static Dictionary<string, CommandLine> CommandMap = new Dictionary<string, CommandLine>();

            static CommandLineEnumMap()
            {
                StrList.Add(BuildTarget);
                StrList.Add(CustomOutputPath);
                CommandMap.Add(BuildTarget, CommandLine.BuildTarget);
                CommandMap.Add(CustomOutputPath, CommandLine.CustomOutputPath);
            }
        }
    }
}