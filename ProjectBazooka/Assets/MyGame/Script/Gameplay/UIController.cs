using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using MyGame.Utilities;
using Sirenix.OdinInspector;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MyGame.Script.Gameplay
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        [SerializeField] private string privacyPolicyLink;

        [Header("Display Button UI")] 
        [SerializeField] public List<RectTransform> displayInteractableUI;
        
        [Header("Popup UI")]
        [SerializeField] public List<RectTransform> popUpUI;

        [Header("Button UI")] 
        [SerializeField] public Button shopBtn;
        
        [Header("Game UI")] 
        [SerializeField] public TMP_Text currentLevelDisplay;
        [SerializeField] public CanvasGroup fadeUI;
        [SerializeField] public TMP_Text cashUIText;
        [SerializeField] public List<GameObject> bulletHolderList;
        [SerializeField] public GameObject bulletHolder;

        [HideInInspector] public int currentUiIsEnable = 0;
        [HideInInspector] public bool isPlayMode;

        private void OnEnable()
        {
            Instance = this;
            bulletHolderList.OnSortFromFirstToLastIndex();
        }

        private void Awake()
        {
            OnUIInit();
            OnButtonShow();
        }
        

        [Button("Asset Preload")]
        async void OnAssetLoad()
        {
            if (FindObjectOfType<ShopManager>(true) != null)
            {
                DestroyImmediate(FindObjectOfType<ShopManager>(true).gameObject);
            }
        }

        public void OnBulletReset()
        {
            foreach (var item in bulletHolderList)
            {
                item.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        
        [Button("Hide Display UI Button")]
        public void OnButtonHide()
        {
            if(isPlayMode) return;
            isPlayMode = true;
            
            foreach (var item in displayInteractableUI)
            {
                var itemPosX = item.anchoredPosition.x;
                item.DOAnchorPosX(-itemPosX, 0.25f);
            }
        }
        [Button("Show Display UI Button")]
        public void OnButtonShow()
        {
            if(!isPlayMode) return;
            isPlayMode = false;
            
            foreach (var item in displayInteractableUI)
            {
                var itemPosX = item.anchoredPosition.x;
                item.DOAnchorPosX(-itemPosX, 0.5f);
            }
        }
        

        void OnUIInit()
        {
            if (GameSettingManager.Instance != null)
            {
                var userData = GameSettingManager.Instance.OnDataLoad();
                if (userData.playerResources.PlayerCurrentLevel < 10)
                {
                    currentLevelDisplay.text = "Level 0" + userData.playerResources.PlayerCurrentLevel;
                }
                else
                {
                    currentLevelDisplay.text = "Level " + userData.playerResources.PlayerCurrentLevel;
                }
                cashUIText.text = userData.playerResources.PlayerCash.ToString();
            }
        }

        public void OnOpenPrivacyPolicy()
        {
            //TODO Change https to privacy policy
            Application.OpenURL(privacyPolicyLink);
        }

        public async void OnPopUpPanelClose(RectTransform rectTransform)
        {
            await fadeUI.DOFade(0f, 0.5f);
            fadeUI.blocksRaycasts = false;
            await rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBounce);
            rectTransform.gameObject.SetActive(false);
            currentUiIsEnable--;
        }
        public void OnPopUpPanelOpen(RectTransform rectTransform)
        {
            rectTransform.gameObject.SetActive(true);
            currentUiIsEnable++;
            fadeUI.blocksRaycasts = true;
            fadeUI.DOFade(0.75f, 0.5f);
            rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        }

        private CancellationTokenSource countCts;
        public void OnUIUpdate(GameData gameData)
        {
            if (countCts != null)
            {
                countCts.Cancel();
            }
            countCts = new CancellationTokenSource();
            var lastCash = Int32.Parse(cashUIText.text);
            DOTween.To(() => lastCash, x => lastCash = x, gameData.playerResources.PlayerCash, 0.5f).OnUpdate(() =>
            {
                cashUIText.text = lastCash.ToString();
            }).WithCancellation(countCts.Token);
            
        }
    }
}
