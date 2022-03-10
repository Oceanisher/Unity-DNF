using System;
using System.Collections;
using System.Collections.Generic;
using Obj;
using Obj.Config.Action.Structure;
using UnityEngine;

namespace Tools
{
    //Gizmos碰撞器绘制工具
    public static class ColDrawUtil
    {
        //地图碰撞器默认颜色
        private static readonly Color MapColColor = Color.gray;
        //OjbCore的Hit类型XZ轴碰撞器颜色
        private static readonly Color ObjHitColColorXZ = Color.red;
        //OjbCore的Judge类型XZ轴碰撞器颜色
        private static readonly Color ObjJudgeColColorXZ = Color.magenta;
        //OjbCore的Body类型XZ轴碰撞器颜色
        private static readonly Color ObjBodyColColorXZ = Color.blue;
        //OjbCore的Hit类型Y轴碰撞器颜色
        private static readonly Color ObjHitColColorY = Color.red;
        //OjbCore的Judge类型Y轴碰撞器颜色
        private static readonly Color ObjJudgeColColorY = Color.magenta;
        //OjbCore的Body类型Y轴碰撞器颜色
        private static readonly Color ObjBodyColColorY = Color.blue;
        //OjbCore的Body类型Y轴碰撞器颜色
        private static readonly Color ObjStaticColColorY = Color.grey;
        
        //绘制地图碰撞器
        public static void DrawMapCol(GameObject obj)
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
            Gizmos.color = MapColColor;

            foreach (var item in colliders)
            {
                Gizmos.DrawWireCube(item.gameObject.transform.position + (Vector3)item.offset, item.size);
            }
        }
        
        //绘制Obj碰撞器
        public static void DrawObjCol(GameObject obj, BoxCollider2D boxCol, PhyCol phyCol)
        {
            if (null == boxCol || null == phyCol)
            {
                return;
            }

            //绘制
            Gizmos.color = GetObjColColor(phyCol);
            
            Gizmos.DrawWireCube(obj.transform.position + (Vector3)boxCol.offset, boxCol.size);
        }

        //绘制角色静态
        public static void DrawOjbColStaticXZ(GameObject obj, BoxCollider2D boxCol)
        {
            //绘制
            Gizmos.color = ObjStaticColColorY;
            
            Gizmos.DrawWireCube(obj.transform.position + (Vector3)boxCol.offset, boxCol.size);
        }

        //获取Obj碰撞器颜色
        private static Color GetObjColColor(PhyCol phyCol)
        {
            if (phyCol.box.colPos == ColPos.XZ)
            {
                switch (phyCol.box.colJudgeType)
                {
                    case ColJudgeType.None:
                        return Color.clear;
                    case ColJudgeType.Hit:
                        return ObjHitColColorXZ;
                    case ColJudgeType.Judge:
                        return ObjJudgeColColorXZ;
                    case ColJudgeType.Body:
                        return ObjBodyColColorXZ;
                }
            }

            if (phyCol.box.colPos == ColPos.Y)
            {
                switch (phyCol.box.colJudgeType)
                {
                    case ColJudgeType.None:
                        return Color.clear;
                    case ColJudgeType.Hit:
                        return ObjHitColColorY;
                    case ColJudgeType.Judge:
                        return ObjJudgeColColorY;
                    case ColJudgeType.Body:
                        return ObjBodyColColorY;
                }
            }
            
            return Color.clear;
        }
    }
}