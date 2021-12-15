using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Config
{
    //行为集合配置
    [CreateAssetMenu(menuName = "Config/Ani&Phy/Behaviours", fileName = "Behaviours")]
    public class BehavioursConfigSo : ScriptableObject
    {
        [Header("默认城镇行为配置")]
        public BehaviourConfigSo defaultBehaviourTown;
        
        [Header("默认战斗行为配置")]
        public BehaviourConfigSo defaultBehaviourFight;
        
        [Header("所有行为配置")]
        public List<BehaviourConfigSo> allBehaviours;

        //AllBehaviours转换为Map
        private Dictionary<BehaviourType, BehaviourConfigSo> _behaviourMap;

        //获取行为
        public BehaviourConfigSo GetByType(BehaviourType type)
        {
            if (null == _behaviourMap)
            {
                GetBehavioursMap();
            }
            _behaviourMap.TryGetValue(type, out var so);
            return so;
        }
        
        //获取行为Map
        private Dictionary<BehaviourType, BehaviourConfigSo> GetBehavioursMap()
        {
            if (null == _behaviourMap)
            {
                SetMap();
            }

            return _behaviourMap;
        }
        
        //生成Map
        private void SetMap()
        {
            _behaviourMap = new Dictionary<BehaviourType, BehaviourConfigSo>();

            if (null == allBehaviours || allBehaviours.Count == 0)
            {
                return;
            }

            foreach (BehaviourConfigSo so in allBehaviours)
            {
                _behaviourMap.Add(so.type, so);
            }
        }
    }
}