using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    //MonoBehaviour单例
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        //互斥锁
        private static readonly object _obj = new object();
        //单例
        private static T _instance;
        //获取单例
        public static T Instance => _instance;
        
        private void Awake()
        {
            if (null != _instance)
            {
                DestroySelf();
                return;
            }

            lock (_obj)
            {
                if (null != _instance)
                {
                    DestroySelf();
                    return;
                }

                _instance = (T) this;
            }
        }

        //重复创建销毁
        private void DestroySelf()
        {
            Log.Error($"[MonoSingleton]单例:{typeof(T)}不可二次创建。", LogModule.Tool);
            Destroy(gameObject);
        }
    }
}