using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyGame.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

public class TransitionControl : MonoBehaviour
{
    public enum TransitionType
    {
        CircleOut,CircleIn,FadeOut,FadeIn
    }

    public TransitionType transitionType;
    public ProceduralImage _proceduralImage;
    [ShowIf("_destroyAnim")]public DestroyAnim _destroyAnim;

    private void OnValidate()
    {
        if (gameObject.HasComponents<DestroyAnim>())
        {
            _destroyAnim = GetComponent<DestroyAnim>();
        }
    }

    [Button("Set Scene Transition")]
    public void OnSet()
    {
        if (transitionType == TransitionType.CircleIn || transitionType == TransitionType.CircleOut)
        {
            _proceduralImage = GetComponent<ProceduralImage>();
            _proceduralImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1000);
            _proceduralImage.ModifierType = typeof(RoundModifier);
            if (transitionType == TransitionType.CircleIn)
            {
                _proceduralImage.BorderWidth = 1;
            }

            if (transitionType == TransitionType.CircleOut)
            {
                _proceduralImage.BorderWidth = 1000;
            }
        }
    }

    [Button("Test Anim")]
    private async void OnEnable()
    {
        if (transitionType == TransitionType.CircleIn || transitionType == TransitionType.CircleOut)
        {
            if (transitionType == TransitionType.CircleIn)
            {
                await DOTween.To(() => _proceduralImage.BorderWidth, x => 
                    _proceduralImage.BorderWidth = x, 1, 0.5f).WithCancellation(this.GetCancellationTokenOnDestroy());
                _destroyAnim.OnDestroyAnimObject();
            }
            if (transitionType == TransitionType.CircleOut)
            {
                await DOTween.To(() => _proceduralImage.BorderWidth, x => 
                    _proceduralImage.BorderWidth = x, 1000, 0.5f).WithCancellation(this.GetCancellationTokenOnDestroy());
                _destroyAnim.OnDestroyAnimObject();
            }
        }
    }
}
