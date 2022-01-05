using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Tools;
using UnityEngine;

namespace Control
{
    //精灵单元，处理精灵切换
    [Obsolete]
    public class SpriteUnit : MonoBehaviour
    {
        [Header("精灵文件夹路径")]
        [SerializeField]
        private string spritePath;

        [Header("精灵文件数量")]
        [SerializeField]
        private int spriteCount;

        [Header("精灵偏移文件")]
        [SerializeField]
        private TextAsset spriteOffsetText;
        
        [Header("每单位像素数量")]
        [SerializeField]
        private int ppu;

        public Vector2 offset;

        //渲染器
        private SpriteRenderer _renderer;
        //实际精灵列表
        private List<Sprite> _spriteList;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            //初始化
            Init();
        }

        //切换精灵
        public void ChangeSprite(int index)
        {
            _renderer.sprite = _spriteList[index];
        }

        //初始化精灵列表
        private void Init()
        {
            //获取偏移量
            _spriteList = new List<Sprite>();
            List<Vector2> offsetList = GetOffset();

            bool isCharacter = offsetList.Count == 1;
            
            for (int i = 0 ; i < spriteCount ; i++)
            {
                //读取精灵，如果为空，那么添加空进来
                Sprite sprite = ResourceUtil.GetSprite(spritePath + "/" + i);
                if (null == sprite)
                {
                    _spriteList.Add(null);
                    continue;
                }
                //如果不为空，那么创建一个精灵
                
                Vector2 pivot = 
                    new Vector2(
                        0.5f - (offsetList[i].x - CoreConstant.DefaultOffset.x + sprite.rect.width / 2) /
                        sprite.rect.width,
                        0.5f + (offsetList[i].y - CoreConstant.DefaultOffset.y + sprite.rect.height / 2) /
                        sprite.rect.height
                    );
                
                Sprite finalSprite = Sprite.Create(
                    sprite.texture,
                    sprite.rect,
                    pivot,
                    ppu);
                
                _spriteList.Add(finalSprite);
            }
            
            //默认精灵为第一个
            _renderer.sprite = _spriteList[0];
        }

        //获取偏移量，从文件中读取
        private List<Vector2> GetOffset()
        {
            List<Vector2> list = new List<Vector2>();
            string [] lineArray = spriteOffsetText.text.Split('\n');
            foreach (var str in lineArray)
            {
                string [] vStr = str.Split(' ');
                Vector2 v = new Vector2(int.Parse(vStr[0]), int.Parse(vStr[1]));
                list.Add(v);
            }

            return list;
        }
    }
}