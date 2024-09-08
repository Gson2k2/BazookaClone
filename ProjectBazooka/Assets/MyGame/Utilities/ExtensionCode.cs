using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace MyGame.Utilities
{
    public static class ExtensionCode
    {
        public static bool HasComponents<T>(this Object obj) where T : Component
        {
            return obj.GetComponent<T>() != null;
        }
        public static bool EventNotNull(this UnityEvent unityEvent)
        {
            if (unityEvent == null)
                return false;
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentTarget(i) != null)
                    return true;
            }
            return false;
        }

        public static void OnSortFromFirstToLastIndex(this List<GameObject> listObj)
        {
            listObj.Sort((a, b) =>
            {
                var aIndex = a.GetComponent<RectTransform>()?.GetSiblingIndex() ?? int.MaxValue;
                var bIndex = b.GetComponent<RectTransform>()?.GetSiblingIndex() ?? int.MaxValue;
                return aIndex.CompareTo(bIndex);
            });
        }
    }
}
