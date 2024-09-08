 using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame.Script.Gameplay.Controller
{
    public class RagdollController : MonoBehaviour
    {
        const string ragdollLayer = "Ragdoll";

        [SerializeField] public List<Rigidbody> ragdollRigidList;
        [SerializeField] public Rigidbody bodyRig;
        [SerializeField] public List<Collider2D> ColTest;
        [SerializeField] public GameObject characterHips;
        [SerializeField] public GameObject characterHeads;
        [SerializeField] public PhysicMaterial PhysicMaterial;

        [Header("Gun")]
        [SerializeField] public GameObject gunBound;
        [SerializeField] public Rigidbody gunRigid;
        [SerializeField] public GameObject gunParent;
        
        [Header("Blood")]
        [SerializeField] public ParticleSystem blood;
        
        [Button("Remove all RigidCol")]
        public void OnRemoveAllRagdoll()
        {
            ragdollRigidList.Clear();
            foreach (var item in GetComponentsInChildren<Rigidbody>())
            {
                DestroyImmediate(item.GetComponent<CharacterJoint>());
                DestroyImmediate(item.GetComponent<Collider>());
                DestroyImmediate(item);
            }
        }
        

        [Button("Set Layers")]
        public void OnDisableKinematic()
        {
            foreach (var item in GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
        [Button("Disable Trigger and Set Tag")]
        public void OnDisableTriggerAndSetTag(string tagName)
        {
            ColTest.Clear();
            foreach (var item in transform.GetComponentsInChildren<Collider>())
            {
                item.tag = "Untagged";
                var itemRb = item.GetComponent<Rigidbody>();
                itemRb.constraints = RigidbodyConstraints.None;
                item.material = PhysicMaterial;
                item.isTrigger = true;
            }
            foreach (var item in transform.GetComponentsInChildren<CharacterJoint>())
            {
                item.enableProjection = true;
            }
            foreach (var item in ragdollRigidList)
            {
                item.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            characterHips.GetComponent<Collider>().isTrigger = false;
            characterHeads.GetComponent<Collider>().isTrigger = false;
            
            if (tagName.IsNullOrWhitespace()) return;
            characterHips.tag = tagName;
            characterHeads.tag = tagName;
        }

        public void OnHolsterWeapon()
        {
            gunBound.gameObject.transform.SetParent(characterHips.transform);
            gunBound.transform.localPosition = new Vector3(0, -0.5f, 0.5f);
        }

        public void OnPreConfig()
        {
            ColTest.Clear();
            characterHips.GetComponent<Collider>().isTrigger = false;
            characterHeads.GetComponent<Collider>().isTrigger = false;
        }
        

        [Button("Enable Projection")]
        public void OnEnableProjection()
        {
            foreach (var item in GetComponentsInChildren<CharacterJoint>())
            {
                item.enableProjection = false;
            }
        }
        
        [Button("Remove Velocity")]
        public void OnRemoveVelocity()
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            foreach (var item in ragdollRigidList)
            {
                item.velocity = Vector3.zero;
            }
        }

        [Button("Active Ragdoll")]
        public void ActiveRagdoll(bool value)
        {
            gunBound.SetActive(!value);
            gunRigid.gameObject.SetActive(value);
            if (value)
            {
                gunRigid.transform.parent = gunParent.transform;
            }
            foreach (var rb in ragdollRigidList)
            {
                rb.isKinematic = !value;
                rb.GetComponent<Collider>().isTrigger = !value;
            }
        }

        
        public void Force(Vector3 pos)
        {
            blood.Play();
            gunRigid.AddExplosionForce(350f,pos,3f);
            foreach(var rb in ragdollRigidList)
            {
                rb.AddExplosionForce(500f,pos,3f);
                //rb.AddForce(new Vector3(5f,5f,0f),ForceMode.Impulse);
            }
        }
        

        [Button("ReValidate")]
        private void ValidateRagdollList()
        {
            ActiveRagdoll(false);
            ragdollRigidList.Clear();
            foreach (var col in transform.GetComponentsInChildren<Collider>())
            {
                col.AddComponent<Rigidbody>();
                ragdollRigidList.Add(col.GetComponent<Rigidbody>());
            }
            foreach (var rb in ragdollRigidList)
            {
                rb.gameObject.layer = LayerMask.NameToLayer(ragdollLayer);
                rb.constraints = RigidbodyConstraints.None;
            }

        }
    }
}
