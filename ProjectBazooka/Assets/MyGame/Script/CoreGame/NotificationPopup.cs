using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class NotificationPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _notiText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;

        public async void OnPopUpNotificationConfig(string title)
        {
            _rectTransform.SetAsFirstSibling();
            _notiText.text = title;
            await DOTween.Sequence()
                .Append(_rectTransform.DOAnchorPosY(800f,1f))
                .Join(_canvasGroup.DOFade(0f,2f))
                .WithCancellation(this.GetCancellationTokenOnDestroy());
            Destroy(gameObject);
        }
    }
}
