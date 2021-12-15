using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //按键工具类
    public class KeyUtil
    {
        private static Array KeyList;

        static KeyUtil()
        {
            KeyList = Enum.GetValues(typeof(KeyCode));
        }

        //获取所有按键列表
        public static Array GetKeyArray()
        {
            return KeyList;
        }
    }
}