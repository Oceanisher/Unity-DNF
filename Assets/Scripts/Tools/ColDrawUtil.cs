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
        private static readonly Color ObjBodyColColorXZ = Color.white;
        //OjbCore的Hit类型Y轴碰撞器颜色
        private static readonly Color ObjHitColColorY = Color.red;
        //OjbCore的Judge类型Y轴碰撞器颜色
        private static readonly Color ObjJudgeColColorY = Color.magenta;
        //OjbCore的Body类型Y轴碰撞器颜色
        private static readonly Color ObjBodyColColorY = Color.white;
        //OjbCore的Body类型Y轴碰撞器颜色
        private static readonly Color ObjStaticColColorY = Color.grey;
        //OjbCore的Body类型Y轴碰撞器颜色
        private static readonly float YInterval = 0.03f;
        
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
        public static void DrawObjCol(GameObject obj, BoxCollider2D boxCol, PhyCol phyCol, Phy phy)
        {
            if (null == boxCol || null == phyCol || null == phy)
            {
                return;
            }

            //绘制
            Gizmos.color = GetObjColColor(phyCol, phy);

            DrawObjColColor(obj, boxCol, phyCol, phy);
        }

        //绘制角色静态
        public static void DrawOjbColStaticXZ(GameObject obj, BoxCollider2D boxCol)
        {
            //绘制
            Gizmos.color = ObjStaticColColorY;
            
            Gizmos.DrawWireCube(obj.transform.position + (Vector3)boxCol.offset, boxCol.size);
        }

        //获取Obj碰撞器颜色
        private static Color GetObjColColor(PhyCol phyCol, Phy phy)
        {
            if (phyCol.colPos == ColPos.XZ)
            {
                switch (phy.judgeType)
                {
                    case PhyJudgeType.None:
                        return Color.clear;
                    case PhyJudgeType.Hit:
                        return ObjHitColColorXZ;
                    case PhyJudgeType.Judge:
                        return ObjJudgeColColorXZ;
                    case PhyJudgeType.Body:
                        return ObjBodyColColorXZ;
                }
            }

            if (phyCol.colPos == ColPos.Y)
            {
                switch (phy.judgeType)
                {
                    case PhyJudgeType.None:
                        return Color.clear;
                    case PhyJudgeType.Hit:
                        return ObjHitColColorY;
                    case PhyJudgeType.Judge:
                        return ObjJudgeColColorY;
                    case PhyJudgeType.Body:
                        return ObjBodyColColorY;
                }
            }
            
            return Color.clear;
        }

        //绘制碰撞器颜色
        private static void DrawObjColColor(GameObject obj, BoxCollider2D boxCol, PhyCol phyCol, Phy phy)
        {
            //如果是Y轴，画一个框
            if (phyCol.colPos == ColPos.Y)
            {
                Gizmos.DrawWireCube(obj.transform.position + (Vector3)boxCol.offset, boxCol.size);
            }
            //如果是XZ轴，画一个带水平线的框
            else
            {
                Vector3 center = obj.transform.position + (Vector3) boxCol.offset;
                Gizmos.DrawWireCube(center, boxCol.size);
                
                /***************碰撞框中画横线***************/
                Vector3 halfSize = boxCol.size / 2;
                Vector3 leftTop = new Vector3(center.x - halfSize.x, center.y + halfSize.y, center.z);
                Vector3 rightBottom = new Vector3(center.x + halfSize.x, center.y - halfSize.y, center.z);
                Vector3 leftBottom = new Vector3(center.x - halfSize.x, center.y - halfSize.y, center.z);
                Vector3 rightTop = new Vector3(center.x + halfSize.x, center.y + halfSize.y, center.z);
                
                float yPos = leftTop.y - YInterval;
                while (yPos > rightBottom.y)
                {
                    Gizmos.DrawLine(
                        new Vector3(leftTop.x, yPos, leftTop.z),
                        new Vector3(rightTop.x, yPos, rightTop.z));
                    yPos -= YInterval;
                }
            }
        }
    }
}