using DG.Tweening;
using MyGame.Script.Gameplay.Controller;
using UnityEngine;

namespace MyGame.Script.TestCode
{
    public abstract class NewCharacter : MonoBehaviour
    {
        public Transform center;
        [SerializeField] public Animator anim;
        [SerializeField] public RagdollController ragdoll;
        protected bool isDead;

        private void Awake()
        {
            OnInit();
        }

        protected virtual void OnInit()
        {
            isDead = false;
            ActiveRagdoll(false);
        }

        public void ActiveRagdoll(bool value)
        {
            anim.enabled = !value;
            ragdoll.ActiveRagdoll(value);
        }

        public virtual void HitBomb(Vector3 force)
        {
            isDead = true;
            DOVirtual.DelayedCall(0.1f, () =>
            {
                ActiveRagdoll(true);
                ragdoll.Force(force);
            });
            
        }
        public virtual void FallDamage()
        {
            isDead = true;
            ragdoll.blood.Play();
            ActiveRagdoll(true);
        }
    }
}
