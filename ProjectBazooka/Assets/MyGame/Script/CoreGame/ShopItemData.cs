using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace MyGame.Script.CoreGame
{
    [CreateAssetMenu(fileName = "ShopData",menuName = "ScriptObject/ItemData",order = 2)]
    public class ShopItemData : ScriptableObject
    {
        public List<ShopData> itemDataArr;
        
        [Serializable]
        public class ShopData
        {
            public int itemID;
            public ItemStatus status;
            public ItemRarity itemRarity;
            public string itemName;
            public int itemCashRequire;
            public GameObject itemModel;
            public Sprite itemModelPreview;
        }
        
        public async void OnAutoGenerateModel(string assetLabelRef)
        {
            List<GameObject> modelArr = new List<GameObject>();
            //TODO Test
            await Addressables.LoadAssetsAsync<GameObject>(assetLabelRef, (obj) =>
            {
                modelArr.Add(obj);
            });
            foreach (var item in itemDataArr)
            {
                item.itemModel =
                    modelArr.First(x => x.name.Equals(item.itemName +"_"+ item.itemID));
            }
        }

        int OnGenerateID()
        {
            var randomID = Random.Range(0, 9999999);
            while(itemDataArr.Any(item => item.itemID == randomID))
            {
                randomID = Random.Range(0, 9999999);
            }

            return randomID;
        }
    }
}
