using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Destructible2D;
using MyGame.Script.CoreGame;
using MyGame.Script.Gameplay.Controller;
using MyGame.Script.TestCode;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyGame.Script.Gameplay
{
    public enum BombType
    {
        Bullet,ExplosionBarrel,NormalObject
    }

    public abstract class ObjectInteractable : MonoBehaviour
    {
        
        [SerializeField] public Collider _collider;
        [SerializeField] public Rigidbody _rigidbody;
        
        public BombType _bombType;
        [HideIf("_bombType",BombType.NormalObject)] public List<GameObject> bombHit;
        [HideIf("_bombType",BombType.NormalObject)] public ParticleSystem explosionPar;


        [HideInInspector] public bool _isExplosion;
        protected bool _isReconstruction;
        protected List<int> _gameObjectID;

        private void Awake()
        {
            _gameObjectID = new List<int>();
        }
        
        protected async void OnExplosionBegan()
        {
            explosionPar.gameObject.SetActive(true);
            explosionPar.transform.parent = null;
            foreach (var item in bombHit)
            {
                item.gameObject.SetActive(true);
            }
        }

        protected async void OnExplosionDestructObj(Collision collision)
        {
            if (collision.gameObject.CompareTag("DestructionObj"))
            {
                if (_gameObjectID.Any(x => x == collision.gameObject.GetInstanceID())) return;
                _gameObjectID.Add(collision.gameObject.GetInstanceID());
                Debug.Log("Object Hit" + collision.gameObject.name + collision.gameObject.GetInstanceID());
                var colRen = collision.gameObject.GetComponent<D2dDestructibleRenderer>().tempSplitDestructible;
                var fixedPos = transform.localPosition;
                if (colRen.IsNullOrEmpty() || colRen.Count <= 0)
                {
                    collision.gameObject.GetComponent<ObjectMeshGen>().OnMeshInstantiate();
                    collision.rigidbody.AddExplosionForce(50f,transform.position,3f);
                }
                else
                {
                    await UniTask.DelayFrame(2);
                    foreach (var item in colRen)
                    {
                        item.gameObject.GetComponent<ObjectMeshGen>().OnMeshInstantiate();
                        item.GetComponent<Rigidbody>().AddExplosionForce(200f,fixedPos,3f);
                    }
                }
            }
            
        }

        protected void OnExplosionDestructPlayer(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Kill: " + collision.gameObject.name);
                var playerRagdollComp = collision.gameObject.GetComponentInParent<NewCharacter>();
                playerRagdollComp.GetComponent<ICharacterInteract>().OnDamageReceive();
                // var forceDis = Vector2.Distance(collision.transform.position, transform.position);
                
                playerRagdollComp.HitBomb(transform.position);
            }
        }

        protected void OnExplosionDestructEnemy(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Kill: " + collision.gameObject.name);
                var enemyRagdollComp = collision.gameObject.GetComponentInParent<RagdollCharacter>();
                enemyRagdollComp.GetComponent<ICharacterInteract>().OnDamageReceive();
                // var forceDis = Vector2.Distance(collision.transform.position, transform.position);
                
                enemyRagdollComp.HitBomb(transform.position);
            }
        }

        protected void OnExplosionDestructGround(Collision collision)
        {
            if (collision.gameObject.CompareTag("Destructable"))
            {
                if (!_isReconstruction)
                {
                    _isReconstruction = true;
                    var meshGen = collision.transform.parent.GetComponent<ObjectMeshGen>();
                    meshGen.OnMeshInstantiate();
                }
            }
        }
        
        protected virtual void OnPlayerFallDamageCollision(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponentInParent<NewPlayerAimController>().OnFallDamageReceive();
            } 
        }
        protected virtual void OnEnemyFallDamageCollision(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponentInParent<EnemyController>().OnFallDamageReceive();

            }
        }
    }
}
