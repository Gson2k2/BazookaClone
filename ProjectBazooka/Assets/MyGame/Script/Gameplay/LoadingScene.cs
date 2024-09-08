using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using MyGame.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyGame.Script.Gameplay
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private Slider _slider;

        private AnimationPool _animationPool;
        private void Start()
        {
            _animationPool = Resources.Load<AnimationPool>("ScriptObject/AnimationPool");
            Instantiate(_animationPool.BG_FadeIn,_canvas.transform);
            OnLoadingTextAnim(this.GetCancellationTokenOnDestroy());
            OnLoadScene(2f);
        }


        async void OnLoadScene(float waitingTime)
        {
            //return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            await DOTween.To(() => _slider.value, x => _slider.value = x, 25, waitingTime/2f);
            await DOTween.To(() => _slider.value, x => _slider.value = x, 100, waitingTime/2f);
            
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);
            if (GameSettingManager.Instance.OnDataLoad().systemData.ShowADS)
            {
                GameSettingManager.Instance.adsManager.OnLoadBanner();
            }
        }

        async void OnLoadingTextAnim(CancellationToken ct)
        {
            var dotCount = 0;
            while (!ct.IsCancellationRequested)
            {
                if(ct.IsCancellationRequested) return;
                await UniTask.Delay(TimeSpan.FromSeconds(1f));

                switch (dotCount % 3)
                {
                    case 0:
                        _loadingText.text = "Loading.";
                        dotCount++;
                        break;
                    case 1:
                        _loadingText.text = "Loading..";
                        dotCount++;
                        break;
                    case 2:
                        _loadingText.text = "Loading...";
                        dotCount = 0;
                        break;
                }
            }
        }
    }
}
