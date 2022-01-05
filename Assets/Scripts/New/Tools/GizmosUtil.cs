using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace New.Tools
{
    //Gizmos工具
    public static class GizmosUtil
    {
        //地图碰撞器默认颜色
        private static readonly Color MapColliderDefaultColor = Color.gray;
        
        //绘制所有碰撞器
        public static void DrawMapCollider(GameObject obj)
        {
            if (null == obj)
            {
                return;
            }

            BoxCollider2D [] colliders = obj.GetComponentsInChildren<BoxCollider2D>();

            if (CollectionUtil.IsEmpty(colliders))
            {
                return;
            }
            
            //绘制
            Gizmos.color = MapColliderDefaultColor;

            foreach (var item in colliders)
            {
                Gizmos.DrawWireCube(item.gameObject.transform.position + (Vector3)item.offset, item.size);
            }
        }
    }
}