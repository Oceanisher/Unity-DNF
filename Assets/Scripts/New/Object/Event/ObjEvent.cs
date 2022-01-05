using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Object.Config;
using UnityEngine;

namespace New.Object.Event
{
    //场景变更事件
    public struct SceneChangeEvent
    {
        //场景变更-变更前
        public SceneType PreSceneType;
        //场景变更-变更后
        public SceneType PostSceneType;
    }

    //行为变更事件
    public struct ActionChangeEvent
    {
        //行为变更-变更前
        public ActionSo PreActionSo;
        //行为变更-变更后
        public ActionSo PostActionSo;
    }

    //帧变更事件
    public struct FrameChangeEvent
    {
        public int PreIndex;
        public int PostIndex;
    }
}