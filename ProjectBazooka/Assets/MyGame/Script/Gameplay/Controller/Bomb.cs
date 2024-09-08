using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using Destructible2D;
using DG.Tweening;
using MyGame.Script.CoreGame;
using MyGame.Script.Gameplay;
using MyGame.Script.Gameplay.Controller;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Debug = UnityEngine.Debug;

public class Bomb : ObjectInteractable
{
    private const string ground = "Ground";
    [HideInInspector] public LevelController _levelController;
    
    public void Shoot(Vector3 force)
    {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(force,ForceMode.Impulse);
    }

    private void Update()
    {
        float angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player")) return;
        if(_isExplosion) return;
        _collider.enabled = false;
        OnExplosionBegan();
        _isExplosion = true;
    }
}
