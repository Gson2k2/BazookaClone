using System.Collections;
using System.Collections.Generic;
using MyGame.Script.CoreGame;
using Sirenix.OdinInspector;
using UnityEngine;

public class FindObject : MonoBehaviour
{
    [SerializeField] private List<Collider2D> _testScript;

    [Button("FindObject")]
    private void OnFindObject()
    {
        _testScript = new List<Collider2D>();
        foreach (var item in GetComponentsInChildren<Collider2D>())
        {
            _testScript.Add(item);
        }
    }
}
