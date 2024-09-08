using System;
using Cysharp.Threading.Tasks;
using MyGame.Script.CoreGame;
using Sirenix.Utilities;
using UnityEngine;

namespace MyGame.Script.Gameplay.Controller
{
    public class EnemyController : RagdollCharacter,ICharacterInteract
    {
        [SerializeField] public LevelController _levelController;
        private Rigidbody _mainBodyRigid;
        private Collider _mainCol;
        
        private void Awake()
        {
            _mainBodyRigid = GetComponent<Rigidbody>();
            _mainCol = GetComponent<Collider>();
            OnInit();
        }
        
        private void Update()
        {
            if(isDead) return;
            if (_mainBodyRigid.velocity.magnitude > 1.25f)
            {
                OnFallDamageReceive();
            }
        }

        public void OnInit()
        {
            isDead = false;
            ActiveRagdoll(false);
            ragdoll.OnPreConfig();
        }

        public async void OnDamageReceive()
        {
            if(isDead) return;
            _levelController.OnAwaitForKillCounter(this);
        }

        public void OnFallDamageReceive()
        {
            if(isDead) return;
            _mainBodyRigid.isKinematic = true;
            _mainCol.enabled = false;
            OnDamageReceive();
            FallDamage();
        }
    }
}
