using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace MyGame.Script.CoreGame
{
    public enum CSVData
    {
        Gun,Skin
    }
    public class CsvRead : MonoBehaviour
    {
        private const string CsvPath = "Assets/MyGame/Resources/ScriptObject/";
        public AssetLabelReference gunAssetData;
        public AssetLabelReference gunAssetIcon;
        [Range(1,6)]public int sheetsColumnCount = 6;

        private TextAsset _textAsset;
        private ShopItemData _shopItemData;
        

        ItemStatus StatusConfig(string req)
        {
            if(req.Equals(ItemStatus.Unavailable.ToString()))
            {
                return ItemStatus.Unavailable;
            }
            if(req.Equals(ItemStatus.CanBuy.ToString()))
            {
                return ItemStatus.CanBuy;
            }
            if(req.Equals(ItemStatus.ADReward.ToString()))
            {
                return ItemStatus.ADReward;
            }
            return ItemStatus.Unavailable;
        }

        ItemRarity RarityConfig(string req)
        {
            if(req.Equals(ItemRarity.Common.ToString()))
            {
                return ItemRarity.Common;
            }
            if(req.Equals(ItemRarity.Rare.ToString()))
            {
                return ItemRarity.Rare;
            }
            if(req.Equals(ItemRarity.Epic.ToString()))
            {
                return ItemRarity.Epic;
            }
            if(req.Equals(ItemRarity.Limited.ToString()))
            {
                return ItemRarity.Limited;
            }
            return ItemRarity.Common;
        }

#if UNITY_EDITOR
        [Button("On Read")]
        public async void OnReadCSV(CSVData csvData)
        {
            string assetName = "null";
            switch (csvData)
            {
                case CSVData.Gun:
                {
                    assetName = "GunShopData";
                    break;
                }
                case CSVData.Skin:
                    assetName = "SkinShopData";
                    break;
                default:
                    Debug.Log("Test");
                    break;
            }
            
            await Addressables.LoadAssetsAsync<TextAsset>(gunAssetData, (textAsset) =>
            {
                _textAsset = textAsset;
            });
                
            string[] stringArr = _textAsset.text.Split(new string[]{",","\n"},StringSplitOptions.None);
            var tableSizeRecalculation = stringArr.Length / 5;

            ShopItemData instanceData = ScriptableObject.CreateInstance<ShopItemData>();
            instanceData.itemDataArr = new List<ShopItemData.ShopData>();
            for (int i = 0; i < tableSizeRecalculation - 1; i++)
            {
                var index = 0;
                instanceData.itemDataArr.Add(new ShopItemData.ShopData()
                {
                    itemID = int.Parse(stringArr[sheetsColumnCount *(i + 1) + index++]),
                    itemName = stringArr[sheetsColumnCount *(i + 1) + index++],
                    itemCashRequire = int.Parse(stringArr[sheetsColumnCount *(i + 1) + index++]),
                    status = StatusConfig(stringArr[sheetsColumnCount *(i + 1) + index++]),
                    itemRarity = RarityConfig(stringArr[sheetsColumnCount *(i + 1) + index++]),
                });
            }

            var itemIcon = new List<Sprite>();
            await Addressables.LoadAssetsAsync<Sprite>(gunAssetIcon, icon =>
            {
                itemIcon.Add(icon);
            });
            Debug.Log(itemIcon.Count);
            foreach (var item in itemIcon)
            {
                Debug.Log(item.name);
            }
            foreach (var item in instanceData.itemDataArr)
            {
                var itemName = item.itemName + "_" + item.itemID;
                Debug.Log(itemName);
                item.itemModelPreview = itemIcon.First(x => x.name.Equals(itemName,StringComparison.OrdinalIgnoreCase));
            }
            
            Debug.Log(instanceData);
            if (AssetDatabase.FindAssets($"{assetName}.asset") != null)
            {
                AssetDatabase.DeleteAsset(CsvPath + $"{assetName}.asset");
                AssetDatabase.SaveAssets();
            }
            AssetDatabase.CreateAsset(instanceData,CsvPath + $"{assetName}.asset");
            AssetDatabase.SaveAssets();

            _shopItemData = AssetDatabase.LoadAssetAtPath<ShopItemData>(CsvPath + $"{assetName}.asset");
            _shopItemData.OnAutoGenerateModel(csvData.ToString());
        }
#endif
    }
    
}
