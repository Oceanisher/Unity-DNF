using New.Core.Config;
using New.Object.Config;
using New.Object.Event;
using New.Tools;
using UnityEngine;

namespace New.Object.Unit
{
    //物体单元抽象类
    public abstract class AbstractObjUnit : MonoBehaviour, IInit
    {
        //是否初始化完成
        public bool HasInit { get; set; }
        
        //物体核心
        protected ObjCore Core;
        
        //初始化
        public virtual void Init(ObjCore objCore)
        {
            Core = objCore;
        }
        //场景变更-前
        public virtual void OnSceneChangePre(SceneChangeEvent changeEvent)
        {}
        //场景变更-后
        public virtual void OnSceneChangePost(SceneChangeEvent changeEvent)
        {}
        //行为变更-前
        public virtual void OnActionChangePre(ActionChangeEvent changeEvent)
        {}
        //行为变更-后
        public virtual void OnActionChangePost(ActionChangeEvent changeEvent)
        {}
        //帧变更-前
        public virtual void OnFrameChangePre(FrameChangeEvent changeEvent)
        {}
        //帧变更-前
        public virtual void OnFrameChangePost(FrameChangeEvent changeEvent)
        {}
    }
}