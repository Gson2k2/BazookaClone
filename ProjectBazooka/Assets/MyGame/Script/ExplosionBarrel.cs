using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyGame.Script.Gameplay;
using MyGame.Script.Gameplay.Controller;
using MyGame.Script.TestCode;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MyGame.Script
{
    public class ExplosionBarrel : ObjectInteractable
    {

        private bool _startExplosion;
        private void Start()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        [Button("Explosion")]
        private void OnExplosion()
        {
            GetComponent<Collider>().enabled = false;
            if (!_isExplosion)
            {
                explosionPar.gameObject.SetActive(true);
                explosionPar.transform.parent = null;
                foreach (var item in bombHit)
                {
                    item.gameObject.SetActive(true);
                }
            }
            _isExplosion = true;
        }

        private async void OnCollisionEnter(Collision collision)
        {
            if (!_startExplosion && collision.gameObject.CompareTag("Bullet"))
            {
                _startExplosion = true;
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                OnExplosion();
            }
            if(_isExplosion) return;
            try
            {
                if (collision.gameObject.CompareTag("Player") && _rigidbody.velocity.magnitude > 1.25)
                {
                    OnPlayerFallDamageCollision(collision);
                }

                if (collision.gameObject.CompareTag("Enemy") && _rigidbody.velocity.magnitude > 1.25)
                {
                    OnEnemyFallDamageCollision(collision);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        protected override void OnPlayerFallDamageCollision(Collision collision)
        {
            base.OnPlayerFallDamageCollision(collision);
            OnExplosion();
        }

        protected override void OnEnemyFallDamageCollision(Collision collision)
        {
            base.OnEnemyFallDamageCollision(collision);
            OnExplosion();
        }
    }
}
