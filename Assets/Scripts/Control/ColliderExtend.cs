using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    //碰撞器扩展
    [Obsolete]
    [RequireComponent(typeof(Collider2D))]
    public class ColliderExtend : MonoBehaviour
    {
        [Header("碰撞器类型")]
        [SerializeField] 
        private ColliderType colliderType;

        public ColliderType ColliderType => colliderType;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }   
}
