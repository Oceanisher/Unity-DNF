using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Tools
{
    //Editor的Gizmos工具，主要用来绘制一些图形
    public static class ObjGizmos
    {
        //绘制Box外框
        public static void DrawActiveBox(BoxCollider2D[] boxArray)
        {
            if (null == boxArray || 0 == boxArray.Length)
            {
                return;
            }
            
            foreach (var box in boxArray)
            {
                DrawActiveBox(box);
            }
        }
        
        //绘制Box外框
        private static void DrawActiveBox(BoxCollider2D box)
        {
            if (!box.isActiveAndEnabled || !box.gameObject.activeInHierarchy)
            {
                return;
            }
            
            //获取其上挂载的Extend
            ColliderExtend extend = box.gameObject.GetComponent<ColliderExtend>();
            if (null == extend)
            {
                Log.Error($"[ObjGizmos]Ojb:{box.gameObject.name}有碰撞器、没有碰撞器扩展。");
                return;
            }
            
            //获取碰撞器类型
            Gizmos.color = GetColliderColor(extend.ColliderType);
            
            Transform boxParentTrs = box.gameObject.GetComponentInParent<Transform>();
            Gizmos.DrawWireCube((Vector2)boxParentTrs.position + box.offset, box.size);
        }

        //绘制脚底的圆圈
        public static void DrawCircle(GameObject obj)
        {
            if (!obj.activeInHierarchy)
            {
                return;
            }
            Gizmos.color = GetMarkColor();
            Gizmos.DrawWireSphere(obj.transform.position, 0.1f);
        }

        //根据碰撞器类型获取对应的颜色
        private static Color GetColliderColor(ColliderType type)
        {
            switch (type)
            {
               case ColliderType.None:
                   return Color.white;
               case ColliderType.Damage:
                   return Color.red;
               case ColliderType.Judge:
                   return Color.blue;
               case ColliderType.Entity:
                   return Color.green;
            }
            Log.Error($"[ObjGizmos]ColliderType:{type}类型没有对应的颜色。");
            return Color.black;
        }

        //获取角色脚底标识的颜色
        private static Color GetMarkColor()
        {
            return Color.yellow;
        }
    }
}
