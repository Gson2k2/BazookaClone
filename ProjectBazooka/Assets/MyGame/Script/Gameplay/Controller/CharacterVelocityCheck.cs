using UnityEngine;

namespace MyGame.Script.Gameplay.Controller
{
    public class CharacterVelocityCheck : MonoBehaviour
    {
        private ICharacterInteract _iCharacterInteract;

        private void Start()
        {
            _iCharacterInteract = GetComponentInParent<ICharacterInteract>();
        }

        private void OnCollisionEnter(Collision col)
        {
            Debug.Log("Enemy Collide with: " +col.gameObject.name);
            if (col.gameObject.CompareTag("DestructionObj"))
            {
                Debug.Log(col.transform.GetComponent<Rigidbody>().velocity.magnitude);
                if (col.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2f)
                {
                    _iCharacterInteract.OnFallDamageReceive();
                }
            }
        }
    }
}
