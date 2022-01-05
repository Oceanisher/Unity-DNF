using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Tools;
using UnityEngine;
using Tools;

namespace New.Core.Module
{
    //中枢接口
    public abstract class AbstractManager<T> : MonoSingleton<T>, IInit where T : MonoSingleton<T>
    {
        //是否初始化完成
        public bool HasInit { get; set; }

        //初始化接口，用于GameManager调用初始化
        public virtual void Init(SceneType type)
        {
        }
        
        //切换场景-之前
        public virtual void ChangeScenePre(SceneType beforeType, SceneType afterType) { }

        //切换场景-完成
        public virtual void ChangeScenePost(SceneType beforeType, SceneType afterType) { }
    }
}