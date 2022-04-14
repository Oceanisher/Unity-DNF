using App.Config;
using UnityEditor;
using UnityEngine;

namespace App.Editor.Build.Tools
{
    //Editor模式下资源获取
    public class EditorResourceUtil : MonoBehaviour
    {
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
    }
}