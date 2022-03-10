using System;

namespace Tools
{
    //UUID生成工具
    public class UuidUtil
    {
        //生成UUID
        public static string Uuid()
        {
            return Guid.NewGuid().ToString();
        }
        
        //比较UUID
        public static bool Compare(string first, string second)
        {
            return string.Equals(first, second);
        }
    }
}