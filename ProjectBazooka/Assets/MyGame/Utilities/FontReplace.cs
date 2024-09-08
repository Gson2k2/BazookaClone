using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyGame.Utilities
{
    public class FontReplace : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private AssetReference _fontAsset;
        [Button("Font Replace")]
        public void OnFontReplace()
        {
            var fontLoad = Addressables.LoadAssetAsync<TMP_FontAsset>(_fontAsset);
            var font = fontLoad.Result;
            
            var textList = FindObjectsOfType<TMP_Text>(true);
            foreach (var text in textList)
            {
                text.font = font;
            }
        }
        
#endif
    }
}
