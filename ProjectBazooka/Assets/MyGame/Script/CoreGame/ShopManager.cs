using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace MyGame.Script.CoreGame
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] public AssetLabelReference assetLabelReference;
        [SerializeField] public Canvas shopCanvas;
        [SerializeField] public RectTransform shopPanel;
        [SerializeField] public List<GameObject> objectModel;
        [Header("ShopItemPanel")]
        [SerializeField] public GameObject gunShop;
        [SerializeField] public GameObject commonSkinShop;
        [SerializeField] public GameObject rareSkinShop;
        [SerializeField] public GameObject epicSkinShop;
        [SerializeField] public GameObject limitedSkinShop;
        [HideInInspector] public ShopItemData shopItemData;

        [SerializeField] private List<Toggle> _toggleList;

        [HideInInspector] public GameData _userTempData;
        [Button("Sort")]
        private void OnSortFromFirstToLastIndex()
        {
            _toggleList.Sort((a,b) =>a.GetComponent<RectTransform>().GetSiblingIndex()
                .CompareTo(b.GetComponent<RectTransform>().GetSiblingIndex()));
        }

        void ToggleValueChanged(Toggle condition)
        {
            if (condition.isOn)
            {
                condition.targetGraphic.color = new Color(0.5f, 0.35f, 1);
            }
            else
            {
                condition.targetGraphic.color = new Color(0.8f, 0.85f, 1);
            }
        }

        void OnUserShopDataReload()
        {
            _userTempData = GameSettingManager.Instance.OnDataLoad();
        }
        private void Awake()
        {
            shopCanvas.worldCamera = Camera.main;
            OnInit();
            // OnShopOpen();
            
            foreach (var item in _toggleList)
            {
                item.onValueChanged.AddListener(delegate { ToggleValueChanged(item); });
            }
        }
        
        async void OnInit()
        {
            shopItemData = Resources.Load<ShopItemData>("ScriptObject/GunShopData");
            _userTempData = GameSettingManager.Instance.OnDataLoad();
            OnUserShopDataReload();
            await Addressables.LoadAssetsAsync<GameObject>(assetLabelReference, (itemData) =>
            {
                var itemComp = itemData.GetComponent<ItemData>();
                foreach (var item in shopItemData.itemDataArr)
                {
                    var itemClone = Instantiate(itemComp, gunShop.transform);
                    itemClone.OnInit(item,this);
                }
            });

        }
        
        public async void OnShopOpen()
        {
            gameObject.SetActive(true);
            UIController.Instance.currentUiIsEnable++;

            UIController.Instance.fadeUI.gameObject.SetActive(true);
            await shopPanel.DOAnchorPosX(0, 0.25f);
            UIController.Instance.fadeUI.DOFade(0.75f, 0.25f);
            objectModel.First().transform.DOScale(Vector3.one, 0.25f);
            objectModel.Last().transform.DOScale(Vector3.one, 0.25f);
        }
        
        public async void OnShopClose()
        {
            await UIController.Instance.fadeUI.DOFade(0f, 0.25f);
            
            UIController.Instance.currentUiIsEnable--;
            UIController.Instance.fadeUI.gameObject.SetActive(false);
            objectModel.First().transform.DOScale(Vector3.zero, 0.25f);
            await objectModel.Last().transform.DOScale(Vector3.zero, 0.25f);
            await shopPanel.DOAnchorPosX(-1500, 0.25f);
            gameObject.SetActive(false);
        }
    }
}
