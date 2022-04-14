using System;
using System.Collections.Generic;
using System.Collections;
using App.Config;
using App.Editor.Build.Tools;
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

            Debug.Log("[DefaultBuilder]CommandLine start.");
            foreach (var item in commandMap)
            {
                Debug.Log($"[DefaultBuilder]CommandLine: {item.Key}, {item.Value}");
            }

            Debug.Log("[DefaultBuilder]CommandLine end.");
            
            //获取配置文件
            AppConfigSo config = EditorResourceUtil.GetAppConfig(BuildTarget.StandaloneOSX);

            //配置版本
            SetPlayerParam(config);

            //打包完整路径，无需带.app
            string fullPath = GenerateMacOutputAppName(commandMap, config);

            BuildPipeline.BuildPlayer(
                GetBuildScenes(),
                fullPath,
                BuildTarget.StandaloneOSX,
                BuildOptions.Development
            );
        }

        //设置Player参数
        private static void SetPlayerParam(AppConfigSo config)
        {
            //配置版本
            PlayerSettings.bundleVersion = config.appVersion;
            PlayerSettings.productName = config.name;
        }

        //获取输出的app的路径+名称，不带.app
        private static string GenerateMacOutputAppName(
            Dictionary<CommandLine, string> commandMap, 
            AppConfigSo config)
        {
            //获取输出路径
            commandMap.TryGetValue(CommandLine.CustomOutputPath, out var outPath);
            //获取编译环境
            commandMap.TryGetValue(CommandLine.PackEnv, out var packEnv);
            
            return outPath + "/" + 
                   config.name + "_" + 
                   PackEnvExtend.GetFromStr(packEnv) + "_" + 
                   config.appVersion + "_" + 
                   TimeUtil.GetTimestampStr();
        }

        //从构建指令中提取参数
        private static Dictionary<CommandLine, string> GetArgs()
        {
            Dictionary<CommandLine, string> argMap = new Dictionary<CommandLine, string>();

            bool isSingle = false;
            CommandLine singleEnum = CommandLine.None;

            //获取指令中的所有参数
            string[] args = System.Environment.GetCommandLineArgs();

            foreach (var arg in args)
            {
                //如果是 "-" 指令
                if (isSingle)
                {
                    argMap.Add(singleEnum, arg);

                    isSingle = false;
                    singleEnum = CommandLine.None;
                    continue;
                }

                //如果是 "--" 指令
                //"--"指令后带等号，所以不需要在下一行处理，本行处理即可
                if (arg.StartsWith("--"))
                {
                    //判断是否有匹配的指令
                    int splitIndex = arg.IndexOf("=");
                    string command = arg.Substring(0, splitIndex);
                    string value = arg.Substring(splitIndex + 1);
                    if (!CommandLineEnumMap.CommandMap.ContainsKey(command))
                    {
                        continue;
                    }

                    CommandLineEnumMap.CommandMap.TryGetValue(command, out var doubleEnum);
                    argMap.Add(doubleEnum, value);
                    continue;
                }

                //非命令，跳过
                if (!CommandLineEnumMap.CommandMap.ContainsKey(arg))
                {
                    continue;
                }

                //如果是 "-" 指令，下一行处理
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

        #region 工具类

        //命令行枚举，目前只列举需要使用的
        private enum CommandLine
        {
            None,

            /*******平台参数*******/
            BuildTarget, //构建平台

            /*******自定义参数*******/
            CustomOutputPath, //输出路径
            PackEnv, //打包环境
        }

        //打包环境枚举
        private enum PackEnv
        {
            None,
            Test, //测试环境
            Stage, //预上线环境
            Product, //线上环境
        }

        //打包环境枚举扩展
        private static class PackEnvExtend
        {
            //根据字符串获取打包环境枚举
            public static PackEnv GetFromStr(string str)
            {
                foreach (PackEnv item in Enum.GetValues(typeof(PackEnv)))
                {
                    if (item.ToString().Equals(str))
                    {
                        return item;
                    }
                }
                return PackEnv.None;
            }
        }

        //命令行与枚举映射
        private static class CommandLineEnumMap
        {
            public static string BuildTarget = "-buildTarget";
            public static string CustomOutputPath = "--outPath";
            public static string PackEnv = "--packEnv";

            public static List<string> StrList = new List<string>();
            public static Dictionary<string, CommandLine> CommandMap = new Dictionary<string, CommandLine>();

            static CommandLineEnumMap()
            {
                StrList.Add(BuildTarget);
                StrList.Add(CustomOutputPath);
                CommandMap.Add(BuildTarget, CommandLine.BuildTarget);
                CommandMap.Add(CustomOutputPath, CommandLine.CustomOutputPath);
                CommandMap.Add(PackEnv, CommandLine.PackEnv);
            }
        }

        #endregion
    }
}