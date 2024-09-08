using UnityEngine;

namespace MyGame.Utilities
{
    [CreateAssetMenu(fileName = "AnimationPool",menuName = "ScriptObject/AnimationPool",order = 1)]
    public class AnimationPool : ScriptableObject
    {
        [SerializeField] public GameObject BG_FadeIn;
        [SerializeField] public GameObject BG_CircleOut;
    }
}
