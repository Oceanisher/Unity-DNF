using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Map
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
            ColDrawUtil.DrawMapCol(bordersGo);
        }

#endif

        #endregion
    }
}