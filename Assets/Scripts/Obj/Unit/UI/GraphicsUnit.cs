using System;
using System.Collections;
using System.Collections.Generic;
using Obj.Config.Action.Structure;
using Obj.Event;
using Sys.Config;
using Tools;
using UnityEngine;

namespace Obj.Unit.UI
{
    //图形单元
    public class GraphicsUnit : AbstractObjUnit
    {
        //精灵渲染器-Body
        private SpriteRenderer _bodyRenderer;

        //动画分片与精灵渲染器映射
        private Dictionary<AniPartType, SpriteRenderer> _rendererMap;
        //分片与精灵渲染器映射
        private Dictionary<AniPartType, List<Sprite>> _spriteMap;

        #region 精灵变更

        //写入精灵
        private void SetSprite(int index)
        {
            if (CollectionUtil.IsEmpty(_rendererMap))
            {
                return;
            }

            foreach (var rendererItem in _rendererMap)
            {
                AniPartType type = rendererItem.Key;
                
                //获取帧配置
                AniFrameInfo frameInfo = Core.GetActiveFrameInfo();

                if (!_spriteMap.TryGetValue(type, out var sprites)
                    || null == frameInfo
                    || frameInfo.spriteSequence >= sprites.Count)
                {
                    continue;
                }
                
                //写入，判断是否水平翻转
                // rendererItem.Value.flipX = !Core.IsPositiveOrientation();
                rendererItem.Value.sprite = sprites[frameInfo.spriteSequence];
            }
        }

        #endregion

        #region 工具方法-写

        //初始化精灵渲染器
        private void InitRenderer()
        {
            _rendererMap = new Dictionary<AniPartType, SpriteRenderer>();
            Transform graphics = gameObject.transform.Find(ActionConstant.Graphics);
            if (null == graphics)
            {
                Log.Error($"[GraphicsUnit]Graphics物体缺失。Obj:{gameObject.name}", LogModule.ObjCore);
                return;
            }
            
            GameObject body = graphics.Find(ActionConstant.Body)?.gameObject;
            GameObject skin = graphics.Find(ActionConstant.Skin)?.gameObject;
            
            //获取使用的Body、皮肤的枚举，Body是必须的
            List<AniPartType> partTypeList;
            if (Core.GlobalGraphics.hasSkin)
            {
                partTypeList = Core.GlobalGraphics.skinUseList;
                if (!partTypeList.Contains(AniPartType.Body))
                {
                    partTypeList.Add(AniPartType.Body);
                }
            }
            else
            {
                partTypeList = new List<AniPartType>();
                partTypeList.Add(AniPartType.Body);
            }
            
            //获取所有物体的精灵渲染器
            foreach (var item in partTypeList)
            {
                //Body处理
                if (item == AniPartType.Body)
                {
                    if (null == body)
                    {
                        Log.Error($"[GraphicsUnit]Body物体缺失。Obj:{gameObject.name}", LogModule.ObjCore);
                        continue;
                    }

                    SpriteRenderer bodyRenderer = body.GetComponent<SpriteRenderer>();
                    if (null == bodyRenderer)
                    {
                        Log.Error($"[GraphicsUnit]Body物体精灵渲染器缺失。Obj:{gameObject.name}", LogModule.ObjCore);
                        continue;
                    }
                    _rendererMap.Add(item, bodyRenderer);
                }
                else
                {
                    //其他皮肤处理
                    if (null == skin)
                    {
                        Log.Error($"[GraphicsUnit]Body皮肤父节点缺失。Obj:{gameObject.name}", LogModule.ObjCore);
                        continue;
                    }

                    SpriteRenderer partRenderer = skin.transform.Find(AniPartTypeExtend.GetName(item))?.GetComponent<SpriteRenderer>();
                    if (null == partRenderer)
                    {
                        Log.Error($"[GraphicsUnit]Body皮肤子节点/精灵渲染器缺失。Obj:{gameObject.name}，Part:{item.ToString()}", LogModule.ObjCore);
                        continue;
                    }
                    _rendererMap.Add(item, partRenderer);
                }
            }
        }
        
        //初始化所有精灵
        private void InitAllSprite()
        {
            _spriteMap = new Dictionary<AniPartType, List<Sprite>>();
            foreach (var pair in _rendererMap)
            {
                //是否跳过皮肤
                if (AniPartTypeExtend.GetAllSkin().Contains(pair.Key)
                    && (!Core.GlobalGraphics.hasSkin || !Core.GlobalGraphics.skinUseList.Contains(pair.Key)))
                {
                    continue;
                }
                
                //获取精灵
                List<Sprite> sprites = Core.GlobalGraphics.GetSprites(pair.Key);
                if (CollectionUtil.IsEmpty(sprites))
                {
                    Log.Error($"[GraphicsUnit]精灵缺失。Type:{pair.Key.ToString()}", LogModule.ObjCore);
                    continue;
                }
                //获取精灵偏移
                List<Vector2Int> spritesOffset = Core.GlobalGraphics.GetSpriteOffset(pair.Key);

                //设置精灵
                List<Sprite> finalSprites = new List<Sprite>();
                for (int i = 0 ; i < sprites.Count; i++)
                {
                    Sprite sprite = sprites[i];
                    if (null == sprite)
                    {
                        Log.Error($"[Graphics]缺失精灵。index:{i},type:{pair.Key}", LogModule.ObjCore);
                        continue;
                    }
                    Vector2Int offset = Vector2Int.zero;
                    if (!CollectionUtil.IsEmpty(spritesOffset)
                        && i < spritesOffset.Count)
                    {
                        offset = spritesOffset[i];
                    }

                    Sprite finalSprite = GenerateSprite(sprite, offset, Core.GlobalGraphics.ppu);
                    finalSprites.Add(finalSprite);
                }
                _spriteMap.Add(pair.Key, finalSprites);
            }
        }

        //生成精灵
        private Sprite GenerateSprite(Sprite sprite, Vector2Int offset, int ppu)
        {
            Vector2Int globalOffset = Core.GlobalGraphics.offset;
            
            Vector2 pivot = 
                new Vector2(
                    0.5f - (offset.x - globalOffset.x + sprite.rect.width / 2) /
                    sprite.rect.width,
                    0.5f + (offset.y - globalOffset.y + sprite.rect.height / 2) /
                    sprite.rect.height
                );
                
            Sprite finalSprite = Sprite.Create(
                sprite.texture,
                sprite.rect,
                pivot,
                ppu);

            return finalSprite;
        }

        //变更朝向
        private void HandleOrientationChange()
        {
            if (CollectionUtil.IsEmpty(_rendererMap))
            {
                return;
            }

            foreach (var rendererItem in _rendererMap)
            {
                //写入，判断是否水平翻转
                rendererItem.Value.flipX = !Core.IsPositiveOrientation();
            }
        }

        #endregion

        #region 事件处理

        //处理事件
        private void HandleEvent(int index)
        {
            if (!HasInit)
            {
                return;
            }
            SetSprite(index);
        }

        #endregion
        
        #region AbstractObjUnit 接口

        public override void Init(ObjCore objCore)
        {
            base.Init(objCore);

            //初始化精灵渲染器
            InitRenderer();
            //初始化所有精灵
            InitAllSprite();

            HasInit = true;
        }
        
        public override void OnFrameChangePost(FrameChangeEvent changeEvent)
        {
            HandleEvent(changeEvent.PostIndex);
        }
        
        public override void OnOrientationChangePost()
        {
            //变更朝向
            HandleOrientationChange();
        }

        #endregion
    }
}