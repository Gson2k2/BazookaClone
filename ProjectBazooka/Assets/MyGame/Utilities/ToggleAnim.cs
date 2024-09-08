using System;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Script.CoreGame;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace MyGame.Utilities
{
    public enum ToggleCondition
    {
        Off,On
    }
    public enum ToggleButtonFunc
    {
        Vsync,ShowAds,Sound,Vibrate,InternetReq
    }
    public class ToggleAnim : MonoBehaviour
    {
        [Header("Toogle Color")] public Color offColor = new Color(0.75f, 1f, 0.75f);
        [Header("Toogle Color")] public Color onColor = new Color(1f, 0.75f, 0.75f);
        [SerializeField] private ToggleCondition _condition;
        [SerializeField] private ToggleButtonFunc _buttonFunc;
        [SerializeField] private Button button;
        private RectTransform _roundTrans;
        private ProceduralImage _backgroundColor;

        private CancellationTokenSource _cancellationTokenSource;
        private void OnValidate()
        {
            if (!gameObject.HasComponents<Button>())
            {
                gameObject.AddComponent<Button>();

            }
            button = GetComponent<Button>();
            OnAddListener();
        }
        [Button("Add Listener - TEST")]
        void OnAddListener()
        {
            MethodInfo targetInfo = UnityEventBase.GetValidMethodInfo(this,
                "OnToggleTrigger", Type.EmptyTypes);
                
            UnityAction unityAction =
                Delegate.CreateDelegate(typeof(UnityAction), this, targetInfo, false) as UnityAction;
            
            #if UNITY_EDITOR
            UnityEventTools.RemovePersistentListener(button.onClick,unityAction);
            UnityEventTools.AddPersistentListener(button.onClick,unityAction);
            #endif
        }

        private void Start()
        {
            if (GameSettingManager.Instance == null) return;
                var userLastConfig = GameSettingManager.Instance.OnDataLoad();
            if (_buttonFunc == ToggleButtonFunc.ShowAds)
            {
                _condition = userLastConfig.systemData.ShowADS ? ToggleCondition.Off : ToggleCondition.On;
            }

            if (_buttonFunc == ToggleButtonFunc.Vsync)
            {
                _condition = userLastConfig.systemData.AppTargetFramerate < (int)Screen.currentResolution.refreshRateRatio.value 
                    ? ToggleCondition.On : ToggleCondition.Off;
            }

            if (_buttonFunc == ToggleButtonFunc.InternetReq)
            {
                if (PlayerPrefs.GetInt("InternetReq") == 1)
                {
                    _condition = ToggleCondition.Off;
                }
                if (PlayerPrefs.GetInt("InternetReq") == 0)
                {
                    _condition = ToggleCondition.On;
                }
            }


            OnToggleTrigger();
        }

        private void Awake()
        {
            _backgroundColor = GetComponent<ProceduralImage>();
            foreach (RectTransform item in gameObject.GetComponentInChildren<RectTransform>())
            {
                if (item != GetComponent<RectTransform>())
                {
                    _roundTrans = item;
                }
            }

            _defRoundPos = _roundTrans.anchoredPosition.x;
        }

        private float _defRoundPos;
        void OnToggleTrigger()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            switch (_condition)
            {
                case ToggleCondition.On:
                    _roundTrans.DOAnchorPosX(_defRoundPos, 0.25f)
                        .SetEase(Ease.Linear)
                        .WithCancellation(_cancellationTokenSource.Token);
                    _backgroundColor.DOColor(offColor, 0.25f)
                        .WithCancellation(_cancellationTokenSource.Token);
                    
                    _condition = ToggleCondition.Off;
                    break;
                case ToggleCondition.Off:
                    _roundTrans.DOAnchorPosX(-_defRoundPos, 0.25f)
                        .SetEase(Ease.Linear)
                        .WithCancellation(_cancellationTokenSource.Token);
                    _backgroundColor.DOColor(onColor, 0.25f)
                        .WithCancellation(_cancellationTokenSource.Token);

                    _condition = ToggleCondition.On;
                    break;
                default:
                    Debug.Log("Tờ ét tét");
                    break;
            }
            GameSettingManager.Instance.OnSystemSettingUpdate(_buttonFunc, _condition);
        }
    }
}
