using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Utilities
{
    public class DestroyAnim : MonoBehaviour
    {
        public bool useChainAnim;
        [ShowIf("useChainAnim")] public GameObject chainAnim;
        public void OnDestroyAnimObject()
        {
            Destroy(gameObject);
        }
    }
}
