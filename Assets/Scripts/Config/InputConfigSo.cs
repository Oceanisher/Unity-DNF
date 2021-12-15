using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Config
{
    //输入配置
    [CreateAssetMenu(menuName = "Config/Input", fileName = "InputConfig")]
    public class InputConfigSo : ScriptableObject
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
        public KeyCode skillFirst;

        //城镇按键与功能映射
        private Dictionary<KeyCode, KeyType> _townKeyMap;
        //战斗按键与功能映射
        private Dictionary<KeyCode, KeyType> _fightKeyMap;
        // //城镇按键配置
        // private List<KeyCode> _townKeys;
        // //战斗按键配置
        // private List<KeyCode> _fightKeys;

        //获取城镇按键对应的功能
        public KeyType GetKeyInTown(KeyCode key)
        {
            if (null == _townKeyMap)
            {
               SetMapTown(); 
            }

            _townKeyMap.TryGetValue(key, out var keyType);
            return keyType;
        }
        
        //获取战斗按键对应的功能
        public KeyType GetKeyInFight(KeyCode key)
        {
            if (null == _fightKeyMap)
            {
                SetMapFight(); 
            }

            _fightKeyMap.TryGetValue(key, out var keyType);
            return keyType;
        }
        
        //
        // //城镇场景是否有该按键
        // public bool HasKeyInTown(KeyCode key)
        // {
        //     return GetAllKeyToTown().Contains(key);
        // }
        //
        // //战斗场景是否有该按键
        // public bool HasKeyInFight(KeyCode key)
        // {
        //     return GetAllKeyToFight().Contains(key);
        // }
        //
        // //获取城镇所有按键配置
        // private List<KeyCode> GetAllKeyToTown()
        // {
        //     if (null == _townKeys)
        //     {
        //         SetList();
        //     }
        //
        //     return _townKeys;
        // }
        //
        // //获取战斗场景所有按键配置
        // private List<KeyCode> GetAllKeyToFight()
        // {
        //     if (null == _fightKeys)
        //     {
        //         SetList();
        //     }
        //
        //     return _fightKeys;
        // }

        // //填充按键List
        // private void SetList()
        // {
        //     _townKeys = new List<KeyCode>();
        //     _fightKeys = new List<KeyCode>();
        //     
        //     _townKeys.Add(up);
        //     _townKeys.Add(down);
        //     _townKeys.Add(left);
        //     _townKeys.Add(right);
        //     
        //     _fightKeys.Add(up);
        //     _fightKeys.Add(down);
        //     _fightKeys.Add(left);
        //     _fightKeys.Add(right);
        //     _fightKeys.Add(hit);
        //     _fightKeys.Add(jump);
        //     _fightKeys.Add(levitate);
        //     _fightKeys.Add(skillFirst);
        // }

        //城镇填充按键映射
        public void SetMapTown()
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
            if (KeyCode.None != skillFirst)
            {
                _townKeyMap.Add(skillFirst, KeyType.SkillFirst);
            }
        }
        
        //战斗填充按键映射
        public void SetMapFight()
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
            if (KeyCode.None != skillFirst)
            {
                _fightKeyMap.Add(skillFirst, KeyType.SkillFirst);
            }
        }
    } 
}
