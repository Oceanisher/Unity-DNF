using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Obj.Config.Action;
using Obj.Config.Action.Structure;
using Obj.Config.Property;
using Sys.Config;
using Sys.Module;
using Tools;
using UnityEngine;

namespace Obj.Config
{
    //物体配置
    [CreateAssetMenu(menuName = "Config/Obj/Config", fileName = "Config")]
    [Serializable]
    public class ObjConfigSo : ScriptableObject
    {
        [Header("物体展示名称")]
        public string objShowName;
        
        [Header("全局属性文件")]
        public TextAsset properties;
        
        [Header("全局图形配置")]
        public GlobalGraphics graphics;
        
        [Header("所有行为列表，包含起始行为、联动行为")]
        public List<ActionSo> allActions;
        
        //TODO 未来要在技能栏配置，现在先放在这里
        [Header("按键与行为映射配置")] 
        public List<DictionaryPair<MapKeyInfo, ActionType>> keyTypeInputMap;

        //获取转换属性配置
        public ObjProperties GetProperties()
        {
            if (null == properties)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ObjProperties>(properties.text);
        }
        
        //是否有按键对应的行为
        //按键判断有优先顺序，技能类优先，移动类后续
        public ActionType GetActionByKeys(List<InputManager.KeyInfo> keyInfoList)
        {
            if (CollectionUtil.IsEmpty(keyTypeInputMap))
            {
                return ActionType.None;
            }

            //对keyType进行排序
            List<InputManager.KeyInfo> tempList = new List<InputManager.KeyInfo>();
            tempList.AddRange(keyInfoList);
            tempList.Sort((a, b) => (int) b.KeyType - (int) a.KeyType);

            foreach (var key in tempList)
            {
                foreach (var pair in keyTypeInputMap)
                {
                    if (key.KeyType == pair.key.type && pair.key.states.Contains(key.KeyState))
                    {
                        return pair.value;
                    }
                }
            }
            
            return ActionType.None;
        }
    }
    
    //字典对
    [Serializable]
    public struct DictionaryPair<T, V>
    {
        public T key;
        public V value;
    }

    //行为与按键映射的Key
    [Serializable]
    public class MapKeyInfo
    {
        [Header("按键类型")]
        public KeyType type;
        [Header("按键状态列表")]
        public List<KeyState> states;
    }
}