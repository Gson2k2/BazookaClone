using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.Utilities
{
    public class PoolManager
    {
        private Queue<MeshCollider> pool = new Queue<MeshCollider>();
        private Transform parentTransform;
        private GameObject emptyObj;

        public PoolManager(Transform parent,GameObject obj)
        {
            parentTransform = parent;
            emptyObj = obj;
        }

        public MeshCollider GetObject()
        {
            if (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                // Instantiate a new object if the pool is empty
                var newObj = Object.Instantiate(emptyObj, parentTransform);
                return newObj.GetComponent<MeshCollider>();
            }
        }
        
        public void ReturnObject(MeshCollider obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
