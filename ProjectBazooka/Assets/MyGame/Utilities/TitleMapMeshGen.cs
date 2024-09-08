using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyGame.Script.CoreGame;
using MyGame.Script.Gameplay.Controller;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

public class TitleMapMeshGen : MeshGenerate
{
    public static TitleMapMeshGen Instance;
    public List<Collider2D> collider2D;
    public float extrusionDepth = 1f;
    

    private void Awake()
    {
        Instance = this;
    }

    private async void OnEnable()
    {
        OnGetAllCollider();
        await MeshFirstGenerate();
    }

    [Button("Get AllCollider")]
    private void OnGetAllCollider()
    {
        collider2D.Clear();
        collider2D.AddRange(transform.GetComponentsInChildren<Collider2D>());
    }
    [Button("Get AllMeshCollider")]
    private void OnGetAllMeshCollider()
    {
        listMeshColArr.Clear();
        listMeshColArr.AddRange(transform.GetComponentsInChildren<MeshCollider>());
    }
    [Button("Clear AllMeshCol")]
    private void OnClearMeshCollider()
    {
        foreach (var item in transform.GetComponentsInChildren<MeshCollider>())
        {
            DestroyImmediate(item.gameObject);
        }
    }
    // [Button("Reconstruct")]
    // public async UniTask MeshReconstruction()
    // {
    //     await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
    //     foreach (var item in modifyCollider)
    //     {
    //         var tempCol = item.collider2D;
    //         Mesh mesh = tempCol.CreateMesh(false, false);
    //         Mesh extrudedMesh = MeshExtrude(mesh, extrusionDepth);
    //     
    //         var cloneObj = Instantiate(emptyGameObj,transform);
    //         MeshCollider meshCollider = cloneObj.AddComponent<MeshCollider>();
    //         meshCollider.sharedMesh = extrudedMesh;
    //         meshCollider.convex = true;
    //         
    //         meshData.Add(new MeshDataList()
    //         {
    //             collider2D = tempCol,
    //             meshRenderer = meshCollider
    //         });
    //         Destroy(item.meshRenderer.gameObject);
    //         meshData.Remove(item);
    //     }
    //     await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
    //     modifyCollider.Clear();
    //     Debug.Log("MeshReconstruction");
    //
    // }

    [SerializeField] private List<MeshCollider> listMeshColArr;

    [Button("Mesh Generate")]
    public async UniTask MeshFirstGenerate()
    {
        OnGetAllCollider();
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        if (!listMeshColArr.IsNullOrEmpty())
        {
            foreach (var item in listMeshColArr)
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                {
                    DestroyImmediate(item);
                }
#endif
                Destroy(item.gameObject);
            }
        }
        listMeshColArr = new List<MeshCollider>();
        foreach (var item in collider2D)
        {
            Mesh mesh = item.CreateMesh(true, true);
            Mesh extrudedMesh = MeshExtrude(mesh, extrusionDepth);

            // var cloneObj = Instantiate(emptyGameObj,transform);
            // MeshCollider meshCollider = cloneObj.AddComponent<MeshCollider>();
            
            // listMeshColArr.Add(meshCollider);
            // meshCollider.sharedMesh = extrudedMesh;
            // meshCollider.convex = true;
        }

        Debug.Log("MeshConstruction");
    }
}
