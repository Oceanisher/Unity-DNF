using System;
using System.Collections;
using System.Collections.Generic;
using New.Core.Config;
using New.Core.Module;
using New.Object.Config.Property;
using New.Tools;
using Newtonsoft.Json;
using UnityEngine;

namespace New.Object.Config
{
    //物体配置
    [CreateAssetMenu(menuName = "Config/Obj/Config", fileName = "Config")]
    [Serializable]
    public class ObjConfigSo : ScriptableObject
    {
        [Header("全局属性文件")]
        public TextAsset properties;
        
        [Header("全局图形配置")]
        public GlobalGraphics graphics;
        
        #region 默认行为列表

        [Header("城镇默认行为，对于非角色类，这是默认行为")]
        public ActionSo townDefaultAction;
        [Header("战斗默认地面行为")]
        public ActionSo fightDefaultGroundAction;
        [Header("战斗默认飞行行为")]
        public ActionSo fightDefaultFlyAction;

        #endregion

        #region 固定行为
        
        [Header("滞空行为")]
        public ActionSo skyStayAction;
        [Header("地面受伤行为")]
        public ActionSo groundHurtAction;
        [Header("地面被控行为")]
        public ActionSo groundControlledAction;
        [Header("空中受伤行为")]
        public ActionSo skyHurtAction;
        [Header("空中被控行为")]
        public ActionSo skyControlledAction;
        [Header("倒地行为")]
        public ActionSo collapseAction;
        [Header("坠落行为")]
        public ActionSo fallAction;
        [Header("起身行为")]
        public ActionSo getUpAction;
        
        #endregion

        #region 技能行为/其他行为

        [Header("行为列表，只包含起始行为")]
        public List<ActionSo> skillActions;

        #endregion
        
        //TODO 未来要在技能栏配置，现在先放在这里
        [Header("按键与行为映射配置")] 
        public List<DictionaryPair<KeyType, ActionSo>> keyTypeInputMap;

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
        public ActionSo GetActionByKeys(List<InputManager.KeyInfo> keyInfoList)
        {
            if (CollectionUtil.IsEmpty(keyTypeInputMap))
            {
                return null;
            }

            //对keyType进行排序
            List<KeyType> tempList = new List<KeyType>();
            foreach (var key in keyInfoList)
            {
                tempList.Add(key.KeyType);
            }
            tempList.Sort((a, b) => (int) a - (int) b);

            foreach (var key in tempList)
            {
                foreach (var pair in keyTypeInputMap)
                {
                    if (key == pair.key)
                    {
                        return pair.value;
                    }
                }
            }
            
            return null;
        }
    }
    
    //字典对
    [Serializable]
    public struct DictionaryPair<T, V>
    {
        public T key;
        public V value;
    }
}