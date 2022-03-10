using Obj.Event;
using Tools;
using UnityEngine;

namespace Obj.Unit
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

        //相当于Update()内部调用，由ObjCore触发
        public virtual void InnerUpdate()
        {}
        //相当于LateUpdate()内部调用，由ObjCore触发
        public virtual void InnerLateUpdate()
        {}
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
        //帧变更-后
        public virtual void OnFrameChangePost(FrameChangeEvent changeEvent)
        {}
        //朝向变更-前
        public virtual void OnOrientationChangePre()
        {}
        //朝向变更-后
        public virtual void OnOrientationChangePost()
        {}
    }
}