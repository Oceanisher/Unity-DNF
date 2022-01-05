using System;
using System.Collections;
using System.Collections.Generic;
using New.Tools;
using UnityEngine;

namespace New.Map
{
    //地图核心
    public class MapCore : MonoBehaviour
    {
        [Header("碰撞器父节点")]
        [SerializeField]
        private GameObject bordersGo;

        #region Editor部分

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            GizmosUtil.DrawMapCollider(bordersGo);
        }

#endif

        #endregion
    }
}