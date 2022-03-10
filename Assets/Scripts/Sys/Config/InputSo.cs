using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sys.Config
{
    //输入配置
    [CreateAssetMenu(menuName = "Config/InputNew", fileName = "KeyMapSo")]
    [Serializable]
    public class InputSo : ScriptableObject
    {
        [Header("移动-上")]
        public KeyCode up;
        [Header("移动-下")]
        public KeyCode down;
        [Header("移动-左")]
        public KeyCode left;
        [Header("移动-右")]
        public KeyCode right;
        [Header("普攻")]
        public KeyCode hit;
        [Header("跳跃")]
        public KeyCode jump;
        [Header("上挑")]
        public KeyCode levitate;
        [Header("技能1")]
        public KeyCode skill1;
        [Header("技能2")]
        public KeyCode skill2;
        [Header("技能3")]
        public KeyCode skill3;
        [Header("技能4")]
        public KeyCode skill4;
        [Header("技能5")]
        public KeyCode skill5;
        [Header("技能6")]
        public KeyCode skill6;
        [Header("技能7")]
        public KeyCode skill7;
        [Header("技能8")]
        public KeyCode skill8;
        [Header("技能9")]
        public KeyCode skill9;
        
        [Header("F1")]
        public KeyCode f1;
        
        //城镇按键与功能映射
        private Dictionary<KeyCode, KeyType> _townKeyMap;
        //战斗按键与功能映射
        private Dictionary<KeyCode, KeyType> _fightKeyMap;

        //根据场景获得按键列表
        public List<KeyCode> GetKeyListByScene(SceneType type)
        {
            return type == SceneType.Town ? GetAllKeyInTown() : GetAllKeyInFight();
        }

        //根据场景获得按键映射
        public Dictionary<KeyCode, KeyType> GetKeyMapByScene(SceneType type)
        {
            return type == SceneType.Town ? GetKeyMapInTown() : GetKeyMapInFight();
        }
        
        //所有按键的KeyCode-城镇
        private Dictionary<KeyCode, KeyType> GetKeyMapInTown()
        {
            if (null == _townKeyMap)
            {
                SetMapTown(); 
            }
            return _townKeyMap;
        }

        //所有按键的KeyCode-战斗
        private Dictionary<KeyCode, KeyType> GetKeyMapInFight()
        {
            if (null == _fightKeyMap)
            {
                SetMapFight(); 
            }
            return _fightKeyMap;
        }

        //所有按键的KeyCode-城镇
        private List<KeyCode> GetAllKeyInTown()
        {
            if (null == _townKeyMap)
            {
                SetMapTown(); 
            }
            return _townKeyMap.Keys.ToList();
        }

        //所有按键的KeyCode-战斗
        private List<KeyCode> GetAllKeyInFight()
        {
            if (null == _fightKeyMap)
            {
                SetMapFight(); 
            }
            return _fightKeyMap.Keys.ToList();
        }
        
        //获取城镇按键对应的功能
        private KeyType GetKeyInTown(KeyCode key)
        {
            if (null == _townKeyMap)
            {
               SetMapTown(); 
            }

            _townKeyMap.TryGetValue(key, out var keyType);
            return keyType;
        }
        
        //获取战斗按键对应的功能
        private KeyType GetKeyInFight(KeyCode key)
        {
            if (null == _fightKeyMap)
            {
                SetMapFight(); 
            }

            _fightKeyMap.TryGetValue(key, out var keyType);
            return keyType;
        }
        
        //城镇填充按键映射
        private void SetMapTown()
        {
            if (null == _townKeyMap)
            {
                _townKeyMap = new Dictionary<KeyCode, KeyType>();
            }
            else
            {
                _townKeyMap.Clear();
            }

            if (KeyCode.None != up)
            {
                _townKeyMap.Add(up, KeyType.Up);
            }
            if (KeyCode.None != down)
            {
                _townKeyMap.Add(down, KeyType.Down);
            }
            if (KeyCode.None != left)
            {
                _townKeyMap.Add(left, KeyType.Left);
            }
            if (KeyCode.None != right)
            {
                _townKeyMap.Add(right, KeyType.Right);
            }
            if (KeyCode.None != hit)
            {
                _townKeyMap.Add(hit, KeyType.Hit);
            }
            if (KeyCode.None != jump)
            {
                _townKeyMap.Add(jump, KeyType.Jump);
            }
            if (KeyCode.None != levitate)
            {
                _townKeyMap.Add(levitate, KeyType.Levitate);
            }
            if (KeyCode.None != f1)
            {
                _townKeyMap.Add(f1, KeyType.F1);
            }
        }
        
        //战斗填充按键映射
        private void SetMapFight()
        {
            if (null == _fightKeyMap)
            {
                _fightKeyMap = new Dictionary<KeyCode, KeyType>();
            }
            else
            {
                _fightKeyMap.Clear();
            }

            if (KeyCode.None != up)
            {
                _fightKeyMap.Add(up, KeyType.Up);
            }
            if (KeyCode.None != down)
            {
                _fightKeyMap.Add(down, KeyType.Down);
            }
            if (KeyCode.None != left)
            {
                _fightKeyMap.Add(left, KeyType.Left);
            }
            if (KeyCode.None != right)
            {
                _fightKeyMap.Add(right, KeyType.Right);
            }
            if (KeyCode.None != hit)
            {
                _fightKeyMap.Add(hit, KeyType.Hit);
            }
            if (KeyCode.None != jump)
            {
                _fightKeyMap.Add(jump, KeyType.Jump);
            }
            if (KeyCode.None != levitate)
            {
                _fightKeyMap.Add(levitate, KeyType.Levitate);
            }
            if (KeyCode.None != skill1)
            {
                _fightKeyMap.Add(skill1, KeyType.Skill1);
            }
            if (KeyCode.None != skill2)
            {
                _fightKeyMap.Add(skill2, KeyType.Skill2);
            }
            if (KeyCode.None != skill3)
            {
                _fightKeyMap.Add(skill3, KeyType.Skill3);
            }
            if (KeyCode.None != skill4)
            {
                _fightKeyMap.Add(skill4, KeyType.Skill4);
            }
            if (KeyCode.None != skill5)
            {
                _fightKeyMap.Add(skill5, KeyType.Skill5);
            }
            if (KeyCode.None != skill6)
            {
                _fightKeyMap.Add(skill6, KeyType.Skill6);
            }
            if (KeyCode.None != skill7)
            {
                _fightKeyMap.Add(skill7, KeyType.Skill7);
            }
            if (KeyCode.None != skill8)
            {
                _fightKeyMap.Add(skill8, KeyType.Skill8);
            }
            if (KeyCode.None != skill9)
            {
                _fightKeyMap.Add(skill9, KeyType.Skill9);
            }
            if (KeyCode.None != f1)
            {
                _fightKeyMap.Add(f1, KeyType.F1);
            }
        }
    }
}