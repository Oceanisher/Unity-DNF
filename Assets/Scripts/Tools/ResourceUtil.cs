using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Config;
using UnityEditor;

namespace Tools
{
    //资源工具
    public class ResourceUtil
    {
        private ResourceUtil() {}

        //获取App配置文件
        public static AppConfigSo GetAppConfig(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                    return Resources.Load<AppConfigSo>("Application/Config/MacAppConfigSo");
                default:
                    return null;
            }
        }
        
        //获取精灵
        public static Sprite GetSprite(string path)
        {
            return Resources.Load<Sprite>(path);
        }
    }
}