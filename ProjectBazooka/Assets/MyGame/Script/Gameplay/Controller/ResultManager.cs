using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using Random = UnityEngine.Random;

namespace MyGame.Script.Gameplay.Controller
{
    public enum RewardBannerType
    {
        LevelComplete,MultiKill,FirstTry
    }
    public class ResultManager : MonoBehaviour
    {
        public static ResultManager Instance;
        private static readonly string LEVEL_COMPLETE_TEXT = "LEVEL CLEAR!";
        private static readonly string LEVEL_KILLING_SPREE_TEXT = "MULTI KILL BONUS!!!";
        private static readonly string LEVEL_FIRST_TRY_TEXT = "First try!";
        [Header("Level Data")]
        [SerializeField] private LevelData _levelData;

        [Header("Other")] 
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button noThanksBtn;
        [SerializeField] private Button playAgainBtn;
        
        [Header("Result Panel")] 
        [SerializeField] private CanvasGroup _resultFadeUI;
        [SerializeField] private GameObject _resultOfferPanel;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _resultFailPanel;
        
        [Header("Task")]
        [SerializeField] private RewardTask _levelCompleteTask;
        [SerializeField] private RewardTask _firstTryTask;
        [SerializeField] private RewardTask _multiKillTask;

        [Header("MoneySpawn Req")]
        [SerializeField] private RectTransform moneyIcoPos;
        [SerializeField] private Sprite moneyIco;
        [SerializeField] private Image emptyProceduralImage;

        private List<RewardTask> _taskQueue;
        private int _moneyCalculated;

        [HideInInspector] public bool isFail;
        private void Awake()
        {
            Instance = this;
            _taskQueue = new List<RewardTask>();
        }

        public void OnResultCalculated(bool offerEnable)
        {
            if(isFail) return;
            _resultFadeUI.gameObject.SetActive(true);
            _resultFadeUI.DOFade(0.75f, 0.25f);
            UIController.Instance.currentUiIsEnable++;
            if (!offerEnable)
            {
                OnResultOfferClose();
            }
            else
            {
                OnResultOfferOpen();
            }
        }

        public void OnLevelFailOpen()
        {
            _resultFadeUI.gameObject.SetActive(true);
            _resultPanel.SetActive(false);
            _resultOfferPanel.SetActive(false);
            _resultFadeUI.DOFade(0.75f, 0.25f);
            _resultFailPanel.SetActive(true);
        }

        public void OnResultFinalClose()
        {
            //TODO Next Level Reset GamePlay Scene
            LevelManager.Instance.OnLevelLoad();
            
            _resultFadeUI.alpha = 0;
            _resultFadeUI.gameObject.SetActive(false);
            UIController.Instance.currentUiIsEnable--;
        }
        public void OnResultOfferClose()
        {
            _resultFailPanel.SetActive(false);
            _resultOfferPanel.SetActive(false);
            _resultPanel.SetActive(true);
        }

        public void OnResultOfferOpen()
        {
            _resultOfferPanel.SetActive(true);
        }

        public void OnQueueReset()
        {
            noThanksBtn.interactable = true;
            playAgainBtn.interactable = true;
            
            _resultOfferPanel.SetActive(false);
            _resultFailPanel.SetActive(false);
            _resultPanel.SetActive(false);
            _levelCompleteTask.gameObject.SetActive(false);
            _firstTryTask.gameObject.SetActive(false);
            _multiKillTask.gameObject.SetActive(false);
            _resultFadeUI.alpha = 0;
            _resultFadeUI.gameObject.SetActive(false);
            
            _taskQueue.Clear();
            _moneyCalculated = 0;
            
            UIController.Instance.currentUiIsEnable = 0;
        }

        public void OnReward()
        {
            var userDataLevel = GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel;
            if (userDataLevel < 10)
            {
                levelText.text = "LEVEL 0"+ GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel +"\nCOMPLETE!";
            }
            else
            {
                levelText.text = "LEVEL "+ GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel +"\nCOMPLETE!";
            }
            levelText.text = "LEVEL "+ GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel +"\nCOMPLETE!";
            foreach (var item in _taskQueue)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void OnQueueResultTask(RewardBannerType bannerType)
        {
            switch (bannerType)
            {
                case RewardBannerType.LevelComplete:
                    if (_taskQueue.IsNullOrEmpty() || _taskQueue.Any(x => x != _levelCompleteTask))
                    {
                        _taskQueue.Add(_levelCompleteTask);
                        _moneyCalculated += _levelData.levelBenefit +
                                            (GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel *
                                             _levelData.levelBenefitPlus);
                    }
                    break;
                case RewardBannerType.MultiKill:
                    if (_taskQueue.IsNullOrEmpty() || _taskQueue.Any(x => x != _multiKillTask))
                    {
                        _taskQueue.Add(_multiKillTask);
                        _moneyCalculated += _levelData.killingSpree;
                    }
                    break;
                case RewardBannerType.FirstTry:
                    if (_taskQueue.IsNullOrEmpty() || _taskQueue.Any(x => x != _firstTryTask))
                    {
                        _taskQueue.Add(_firstTryTask);
                        _moneyCalculated += _levelData.levelFirstTry;

                    }
                    break;
            }

            OnBenefitCalculator(bannerType);
        }
        
        public void OnBenefitCalculator(RewardBannerType bannerType)
        {
            switch (bannerType)
            {
                case RewardBannerType.LevelComplete:
                {
                    var benefitCalculator = _levelData.levelBenefit +
                                            (GameSettingManager.Instance.OnDataLoad().playerResources.PlayerCurrentLevel *
                                             _levelData.levelBenefitPlus);
                    _levelCompleteTask.OnRewardTaskConfig("+"+benefitCalculator.ToString(), LEVEL_COMPLETE_TEXT);
                    break;
                }
                case RewardBannerType.FirstTry:
                {
                    _firstTryTask.OnRewardTaskConfig("+"+_levelData.levelFirstTry.ToString(), LEVEL_FIRST_TRY_TEXT);
                    break;
                }
                case RewardBannerType.MultiKill:
                {
                    _multiKillTask.OnRewardTaskConfig("+"+_levelData.killingSpree.ToString(), LEVEL_KILLING_SPREE_TEXT);
                    break;
                }
            }
        }
        
        public async void OnPlayAgainCalculate(Button button)
        {
            button.interactable = false;
            OnQueueReset();
            isFail = false;

            UIController.Instance.OnButtonShow();
            LevelManager.Instance.OnLevelLoad();
        }

        public async void OnNoThanksCalculate(Button button)
        {
            if(isFail) return;
            
            button.interactable = false;
            GameSettingManager.Instance.OnDataCashUpdate(_moneyCalculated);
            GameSettingManager.Instance.OnPlayerLevelUpdate();
            await OnMoneySpawn();
            
            UIController.Instance.OnButtonShow();

            OnResultFinalClose();
            OnQueueReset();
        }
        [Button("Spawn Money")]
        private async UniTask OnMoneySpawn()
        {
            for (int i = 0; i < 10; i++)
            {
                var cloneImg = Instantiate(emptyProceduralImage, transform);
                cloneImg.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                cloneImg.sprite = moneyIco;
                Vector3 worldPosition = moneyIcoPos.TransformPoint(moneyIcoPos.rect.center);
                
                DOTween.Sequence()
                    .Append(cloneImg.transform.DOScale(Vector3.one, 0.25f))
                    .Append(cloneImg.transform.DOMove(worldPosition, 0.5f))
                    .OnComplete(() =>
                    {
                        Destroy(cloneImg.gameObject);
                    }).WithCancellation(cloneImg.GetCancellationTokenOnDestroy());
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

        }
    }
}
