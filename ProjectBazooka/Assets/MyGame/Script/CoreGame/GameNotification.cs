using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public enum NotificationType
    {
        InternetLost,MoneyInsufficient,RewardCancel,ItemPurchase,ItemAcquired
    }
    public class GameNotification : MonoBehaviour
    {
        public static GameNotification Instance;
        private static readonly string DEFAULT_INTERNET_LOST_CONNECT_TEXT = "No Internet Connection,Please try again!";
        private static readonly string DEFAULT_MONEY_INSUFFICIENT_TEXT = "Not Enough Money!";
        private static readonly string DEFAULT_ADS_REWARD_CANCEL_TEXT = "Reward Cancel!";
        private static readonly string DEFAULT_PURCHASE_ITEM_CANCEL_TEXT = "Purchase Successfully!";
        private static readonly string DEFAULT_ITEM_ACCQUIRED_CANCEL_TEXT = "Item Acquired!";

        [SerializeField] private NotificationPopup _notificationPopup;
        [SerializeField] private RectTransform internetLostPanel;
        [SerializeField] private Animator loadingAnim;
        [SerializeField] private CanvasGroup fadeUI;
        
        [Header("Notification Text")]
        [TextArea(1,2),ValidateInput("ValidateStringLength", "InternetNotificationText length must be 50 characters or less.")]
        [SerializeField] private string _internetNotificationText;
        [TextArea(1,2),ValidateInput("ValidateStringLength", "InsufficientMoneyNotificationText length must be 50 characters or less.")]
        [SerializeField] private string _insufficientMoneyNotificationText;
        [TextArea(1,2),ValidateInput("ValidateStringLength", "ADSRewardCancelNotificationText length must be 50 characters or less.")]
        [SerializeField] private string _adsRewardCancelNotificationText;
        [TextArea(1,2),ValidateInput("ValidateStringLength", "PurchaseItemNotificationText length must be 50 characters or less.")]
        [SerializeField] private string _purchaseItemNotificationText;
        [TextArea(1,2),ValidateInput("ValidateStringLength", "PurchaseItemNotificationText length must be 50 characters or less.")]
        [SerializeField] private string _acquiredItemNotificationText;

        CancellationTokenSource _InternetCancellation;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            _InternetCancellation = new CancellationTokenSource();
        }

        private void Start()
        {
            CheckInternetRoutine().Forget();
        }
        private async UniTaskVoid CheckInternetRoutine()
        {
            while (PlayerPrefs.GetInt("InternetReq") == 1)
            {
                await UniTask.Delay((int)(_internetInterval * 1000));
                await CheckInternetConnection();
            }
        }

        private bool ValidateStringLength(string value)
        {
            if (value.Length > 50)
            {
                _internetNotificationText = value.Substring(0, 50);
                return false;
            }
            return true;
        }

        private float _internetInterval = 5; // Check every 5 seconds
        private float _nextCheckTime = 0f;
        private bool _isChecking;

        private async UniTask OnInternetLostConnectPanel()
        {
            _InternetCancellation?.Cancel();
            _InternetCancellation = new CancellationTokenSource();
            
            if (PlayerPrefs.GetInt("InternetReq") == 1)
            {
                internetLostPanel.gameObject.SetActive(true);
                loadingAnim.enabled = true;
                fadeUI.gameObject.SetActive(true);
                fadeUI.DOFade(0.5f, 0.25f);
                await internetLostPanel.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .WithCancellation(_InternetCancellation.Token);
            
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
                
            }
            
            var checkInternet = await InternetWebRequest(); 

            if (PlayerPrefs.GetInt("InternetReq") == 0)
            {
                checkInternet = true;
            }
            if (checkInternet)
            {
                await internetLostPanel.DOScale(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .WithCancellation(_InternetCancellation.Token);
                fadeUI.DOFade(0.5f, 0.25f);
                loadingAnim.enabled = false;
                fadeUI.gameObject.SetActive(false);
                
                internetLostPanel.gameObject.SetActive(false);
                _isChecking = false;
            }
            else
            {
                await OnInternetLostConnectPanel();
            }
        }
        

        private async UniTask CheckInternetConnection()
        {
            if(_isChecking) return;
            _isChecking = true;
            bool isConnected = await InternetWebRequest();
            Debug.Log(isConnected ? "Internet is available." : "No internet connection.");

            if (!isConnected)
            {
                await OnInternetLostConnectPanel();
            }
        }
        
        private async UniTask<bool> InternetWebRequest()
        {
            try
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("http://google.com");
                _isChecking = false;
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        [Button("Reset Notification Text")]
        private void OnResetNotificationText()
        {
            _internetNotificationText = DEFAULT_INTERNET_LOST_CONNECT_TEXT;
            _insufficientMoneyNotificationText = DEFAULT_MONEY_INSUFFICIENT_TEXT;
            _adsRewardCancelNotificationText = DEFAULT_ADS_REWARD_CANCEL_TEXT;
            _purchaseItemNotificationText = DEFAULT_PURCHASE_ITEM_CANCEL_TEXT;
            _acquiredItemNotificationText = DEFAULT_ITEM_ACCQUIRED_CANCEL_TEXT;
        }

        [Button("Notification Test")]
        public void OnNotiTest(NotificationType notificationType)
        {
            OnNotificationPopUp(notificationType);
        }

        public void OnNotificationPopUp(NotificationType notificationType)
        {
            var notificationPopupClone =  Instantiate(_notificationPopup, transform);
            switch (notificationType)
            {
                case NotificationType.InternetLost:
                    notificationPopupClone.OnPopUpNotificationConfig(_internetNotificationText);
                    break;
                case NotificationType.MoneyInsufficient:
                    notificationPopupClone.OnPopUpNotificationConfig(_insufficientMoneyNotificationText);
                    break;
                case NotificationType.RewardCancel:
                    notificationPopupClone.OnPopUpNotificationConfig(_adsRewardCancelNotificationText);
                    break;
                case NotificationType.ItemPurchase:
                    notificationPopupClone.OnPopUpNotificationConfig(_purchaseItemNotificationText);
                    break;
                case NotificationType.ItemAcquired:
                    notificationPopupClone.OnPopUpNotificationConfig(_acquiredItemNotificationText);
                    break;
            }
        }
    }
}
