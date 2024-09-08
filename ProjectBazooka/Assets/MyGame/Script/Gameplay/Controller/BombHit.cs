using System;
using System.Collections.Generic;
using System.Linq;
using Destructible2D;
using MyGame.Script.CoreGame;
using UnityEditor;
using UnityEngine;

namespace MyGame.Script.Gameplay.Controller
{
    public class BombHit : ObjectInteractable
    {
        public GameObject explosionParent;

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Explosionn!!");
            OnExplosionDestructObj(collision);
            OnExplosionDestructGround(collision);
            OnExplosionDestructPlayer(collision);
            OnExplosionDestructEnemy(collision);
            Destroy(explosionParent.gameObject);
        }
    }
}
