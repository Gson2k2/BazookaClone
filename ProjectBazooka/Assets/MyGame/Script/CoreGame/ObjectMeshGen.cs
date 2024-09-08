using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MyGame.Utilities;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public class ObjectMeshGen : MonoBehaviour
    {
        public GameObject emptyObj;
        public List<PolygonCollider2D> polyArr;
        public float depth = 10f;

        private PoolManager _poolManager;
        private void Awake()
        {
            polyArr = new List<PolygonCollider2D>();
            _poolManager = new PoolManager(transform, emptyObj);
            OnMeshInstantiate();
        }

        void OnGetCol()
        {
            polyArr = GetComponentsInChildren<PolygonCollider2D>().ToList();
        }
        void OnClearMesh()
        {
            foreach (var item in GetComponentsInChildren<MeshCollider>())
            {
                _poolManager.ReturnObject(item);
            }
        }
        
        
        private CancellationTokenSource _cancellationTokenSource;
        // private bool _isFirstTime;
        [Button("Mesh Gen")]
        public async void OnMeshInstantiate()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // if (!_isFirstTime)
                // {
                //     await Task.Delay(TimeSpan.FromSeconds(0.5f), _cancellationTokenSource.Token);
                // }
                // _isFirstTime = true;
                OnClearMesh();
                OnGetCol();
                NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(polyArr.Count, Allocator.Temp);

                for (int i = 0; i < polyArr.Count; i++)
                {
                    var item = polyArr[i];
                    var cloneObj = _poolManager.GetObject();
                
                    cloneObj.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, 0);
                    cloneObj.transform.localScale = new Vector3(item.transform.localScale.x, item.transform.localScale.y,
                        item.transform.localScale.z);

                    MeshFilter meshFilter = cloneObj.GetComponent<MeshFilter>();
                    MeshCollider meshCollider = cloneObj.GetComponent<MeshCollider>();

                    NativeArray<Vector3> vertices = new NativeArray<Vector3>(item.points.Length * 2, Allocator.TempJob);
                    NativeArray<int> triangles = new NativeArray<int>(item.points.Length * 6, Allocator.TempJob);

                    MeshCreationJob meshJob = new MeshCreationJob
                    {
                        points = new NativeArray<Vector2>(item.points, Allocator.TempJob),
                        depth = depth,
                        vertices = vertices,
                        triangles = triangles
                    };
                    jobHandles[i] = meshJob.Schedule();
                    jobHandles[i].Complete();

                    Mesh mesh = new Mesh();
                    mesh.vertices = vertices.ToArray();
                    mesh.triangles = triangles.ToArray();
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    meshFilter.mesh = mesh;
                    meshCollider.sharedMesh = mesh;

                    vertices.Dispose();
                    triangles.Dispose();
                    meshJob.points.Dispose();
                }
                jobHandles.Dispose();
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Cancel");
                return;
            }
            Debug.Log("Mesh Reconstruction");
            
        }

        [BurstCompile]
        struct MeshCreationJob : IJob
        {
            [Unity.Collections.ReadOnly] public NativeArray<Vector2> points;
            public float depth;

            public NativeArray<Vector3> vertices;
            public NativeArray<int> triangles;

            public void Execute()
            {
                int pointCount = points.Length;

                for (int i = 0; i < pointCount; i++)
                {
                    vertices[i] = new Vector3(points[i].x, points[i].y, -depth / 2);
                    vertices[i + pointCount] = new Vector3(points[i].x, points[i].y, depth / 2);
                }

                for (int i = 0; i < pointCount; i++)
                {
                    int nextIndex = (i + 1) % pointCount;

                    triangles[i * 6] = i;
                    triangles[i * 6 + 1] = nextIndex;
                    triangles[i * 6 + 2] = i + pointCount;

                    triangles[i * 6 + 3] = nextIndex;
                    triangles[i * 6 + 4] = nextIndex + pointCount;
                    triangles[i * 6 + 5] = i + pointCount;
                }
            }
        }
    }
}

