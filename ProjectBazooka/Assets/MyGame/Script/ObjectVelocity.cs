using System;
using MyGame.Script.Gameplay;
using UnityEngine;

namespace MyGame.Script
{
    public class ObjectVelocity : ObjectInteractable
    {
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private async void OnCollisionEnter(Collision collision)
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
    }
}
