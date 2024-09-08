using System;
using System.Collections;
using System.Collections.Generic;
using MyGame.Script.Gameplay.Controller;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (transform.parent.localPosition.y > other.transform.position.y)
            {
                other.GetComponentInParent<RagdollCharacter>().anim.Play("Hunter_Scare");
            }
        }
    }
}
