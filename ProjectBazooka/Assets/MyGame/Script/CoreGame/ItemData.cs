using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace MyGame.Script.CoreGame
{
    public class ItemData : MonoBehaviour
    {
        private static readonly Color COMMON_COLOR = new Color(0.902f, 1, 0.922f,255);
        private static readonly Color RARE_COLOR = new Color(0.522f, 0.698f, 1,255);
        private static readonly Color EPIC_COLOR = new Color(0.827f, 0.549f, 1,255);
        private static readonly Color LIMITED_COLOR = new Color(1, 0.545f, 0.090f,255);

        public Image itemIcon;
        public ItemStatus itemStatus;
        public ItemRarity itemRarity;
        [Header("Item Panel")]
        public GameObject moneyReqPanel;
        public GameObject adsReqPanel;
        public GameObject unavailableReqPanel;
        [Header("Another Config")]
        public TMP_Text priceText;
        public ProceduralImage itemColor;

        private ShopManager _shopManager;

        private int _itemID;
        private bool _isObtained;
        public void OnInit(ShopItemData.ShopData itemData,ShopManager shopManager)
        {
            _shopManager = shopManager;

            itemIcon.sprite = itemData.itemModelPreview;
            itemStatus = itemData.status;
            itemRarity = itemData.itemRarity;
            _itemID = itemData.itemID;
            priceText.text = itemData.itemCashRequire.ToString();

            OnItemStatusPanelConfig();
            OnRarityColorCheck();
        }

        private void OnItemStatusPanelConfig()
        {
            if (_shopManager._userTempData.itemData.itemID.Any(x => x == _itemID))
            {
                OnItemObtain();
                return;
            }
            switch (itemStatus)
            {
                case ItemStatus.CanBuy:
                    moneyReqPanel.SetActive(true);
                    break;
                case ItemStatus.ADReward:
                    adsReqPanel.SetActive(true);
                    break;
                case ItemStatus.Unavailable:
                    unavailableReqPanel.SetActive(true);
                    break;
            }
        }

        private void OnRarityColorCheck()
        {
            switch (itemRarity)
            {
                case ItemRarity.Common:
                    itemColor.color = COMMON_COLOR;
                    break;
                case ItemRarity.Rare:
                    itemColor.color = RARE_COLOR;
                    break;
                case ItemRarity.Epic:
                    itemColor.color = EPIC_COLOR;
                    break;
                case ItemRarity.Limited:
                    itemColor.color = LIMITED_COLOR;
                    break;
            }
        }

        public void OnItemObtain()
        {
            _isObtained = true;
            moneyReqPanel.SetActive(false);
            adsReqPanel.SetActive(false);
            unavailableReqPanel.SetActive(false);
        }

        public void OnDeselected()
        {
            
        }

        public void OnSelected()
        {
            if (_isObtained)
            {
                Debug.Log("Equip Item ID" + _itemID);
            }
            else
            {
                if (itemStatus == ItemStatus.CanBuy)
                {
                    var userData = GameSettingManager.Instance.OnDataLoad();
                    var itemQuery = _shopManager.shopItemData.itemDataArr.First(x => x.itemID == _itemID);
                    if (userData.playerResources.PlayerCash >= itemQuery.itemCashRequire)
                    {
                        _isObtained = true;
                        GameSettingManager.Instance.OnDataCashUpdate(-itemQuery.itemCashRequire);
                        GameSettingManager.Instance.OnItemObtainUpdate(_itemID);
                    }
                }

                if (itemStatus == ItemStatus.ADReward)
                {
                    //TODO Ads reward
                }

            }
        }
    }
}
