using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //资源工具
    public static class ResourceUtil
    {
        //获取精灵
        public static Sprite GetSprite(string path)
        {
            return Resources.Load<Sprite>(path);
        }
    }
}