using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGame.Script.Gameplay.Controller
{
    public class RagdollCharacter : MonoBehaviour
    {
        [SerializeField] public Animator anim;
        [SerializeField] protected RagdollController ragdoll;
        protected bool isDead;
        
        public void ActiveRagdoll(bool value)
        {
            anim.enabled = !value;
            // rb.isKinematic = !value;
            if (ragdoll != null)
            {
                ragdoll.ActiveRagdoll(value);
            }
            
        }
        [Button("Bomb Hit")]
        public virtual void HitBomb(Vector3 pos)
        {
                Debug.Log("Hit Bomm");
            // anim.Play(die);
            // bloodFx.Play();
            isDead = true;
            if (ragdoll != null)
            {
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    ActiveRagdoll(true);
                    ragdoll.Force(pos);
                });
            }
            
        }
        public void FallDamage()
        {
            isDead = true;
            ragdoll.blood.Play();
            ActiveRagdoll(true);
        }
        
        [Button("RagdollActive")]
        private void OnActiveRagdoll()
        {
            ActiveRagdoll(true);
        }


        protected virtual void OnValidate()
        {
            if (!anim)
            {
                anim = GetComponentInChildren<Animator>();
            }

            // if (!bodyCollider)
            // {
            //     bodyCollider = GetComponent<Collider2D>();
            // }

            // if (!ragdoll)
            // {
            //     ragdoll = GetComponent<RagdollController>();
            // }

        }
    }
}
