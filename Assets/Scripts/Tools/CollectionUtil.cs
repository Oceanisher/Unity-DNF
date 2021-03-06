using System.Collections;
using System.Collections.Generic;

namespace Tools
{
    //集合工具类
    public static class CollectionUtil
    {
        //是否为空
        public static bool IsEmpty<T>(T collection) where T : ICollection
        {
            return null == collection || collection.Count == 0;
        }
        
        //是否为空
        public static bool IsEmpty<T>(ISet<T> collection)
        {
            return null == collection || collection.Count == 0;
        }
    }
}