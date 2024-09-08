using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.Gameplay;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Script.CoreGame
{
    public class DevBuildUtil : MonoBehaviour
    {
        public static DevBuildUtil Instance;
        public GameObject devBuildWindow;
        public Button devUiBtn;
        public CanvasGroup fadeUI;
        public TMP_Text fpsCounter;
        public RectTransform devBuildWindowPanel;

        [Header("Ads Dev")] 
        public GameObject adsPanel;

        [SerializeField] public bool debugMode;
        [HideInInspector] public bool devPanelEnable;

        private void OnEnable()
        {
            Instance = this;
            if (!PlayerPrefs.HasKey("InternetReq"))
            {
                PlayerPrefs.SetInt("InternetReq",1);
            }
            //debugMode = Debug.isDebugBuild;
            
            //TODO use Debug.isDebugBuild later instead of !Debug.isDebugBuild
            
            if (!debugMode)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                devUiBtn.onClick.AddListener(OnDebugCommandShowUp);
            }
        }

        public void OnMoneyAdd()
        {
            GameSettingManager.Instance.OnDataCashUpdate(5000);
        }

        public void OnDataClearAll()
        {
            GameSettingManager.Instance.OnDataReset();
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }

        private void Update()
        {
            var fpsRead = (int)(1f / Time.deltaTime);
            fpsCounter.text = fpsRead.ToString();
        }

        public async void OnCloseDebugCommand()
        {
            
            fadeUI.DOFade(0f, 0.25f)
                .WithCancellation(this.GetCancellationTokenOnDestroy());
            await devBuildWindowPanel.DOScale(0f,0.5f).SetEase(Ease.InBounce)
                .WithCancellation(this.GetCancellationTokenOnDestroy());
            fadeUI.gameObject.SetActive(false);
            devPanelEnable = false;
        }

        async void OnDebugCommandShowUp()
        {
            devPanelEnable = true;
            if (!GameSettingManager.Instance.OnDataLoad().systemData.ShowADS)
            {
                adsPanel.SetActive(false);
            }
            fadeUI.gameObject.SetActive(true);
            await DOTween.Sequence()
                .Append(fadeUI.DOFade(0.5f, 0.25f))
                .AppendCallback(() =>
                {
                    devBuildWindow.SetActive(true);
                })
                .Append(devBuildWindowPanel.DOScale(new Vector3(1f,1f,1f),0.5f).SetEase(Ease.OutBounce))
                .WithCancellation(this.GetCancellationTokenOnDestroy());
        }
    }
}
