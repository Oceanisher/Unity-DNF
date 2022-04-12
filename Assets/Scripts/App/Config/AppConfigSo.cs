using System;
using UnityEngine;

namespace App.Config
{
    //Application配置
    [CreateAssetMenu(menuName = "Config/Sys/AppConfig", fileName = "AppConfigSo")]
    [Serializable]
    public class AppConfigSo : ScriptableObject
    {
        [Header("App名称")]
        public string appName;

        [Header("App版本")]
        public string appVersion;
    }
}