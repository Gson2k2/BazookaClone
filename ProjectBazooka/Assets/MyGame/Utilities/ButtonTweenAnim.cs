using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Utilities
{
    public class ButtonTweenAnim : MonoBehaviour,IButtonInteractive
    {
        private Button _button;
        private RectTransform _buttonRectTrans;
        CancellationTokenSource _cancellationToken;

        private Vector2 btnScale;
        private void OnValidate()
        {
            if (!gameObject.HasComponents<Button>())
            {
                gameObject.AddComponent<Button>();
            }
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _buttonRectTrans = GetComponent<RectTransform>();
            _button.onClick.AddListener(OnClickAnimation);
        }

        private void Start()
        {
            btnScale = _buttonRectTrans.sizeDelta;
        }
        

        void OnClickAnimation()
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();
            DOTween.Sequence()
                .Append(_buttonRectTrans.DOSizeDelta(new Vector2(btnScale.x*1.5f,btnScale.y*1.5f), 0.25f))
                .Append(_buttonRectTrans.DOSizeDelta(new Vector2(btnScale.x,btnScale.y), 0.25f))
                .SetEase(Ease.OutBounce)
                .WithCancellation(_cancellationToken.Token);
        }

        private void OnDestroy()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }
    }

    public interface IButtonInteractive
    {
        public virtual void OnEventTrigger(){}
    }
}
