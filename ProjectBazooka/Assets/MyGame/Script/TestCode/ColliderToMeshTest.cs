using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public class MeshGenerator : MonoBehaviour
{
    public GameObject emptyObj;
    public List<PolygonCollider2D> polyArr;
    public float depth = 1f;

    public void OnGetCol()
    {
        polyArr = GetComponentsInChildren<PolygonCollider2D>().ToList();
    }
    
    [Button("Mesh Gen")]
    async void OnMeshInstantiate()
    {
        OnGetCol();
        await UniTask.DelayFrame(1);
        NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(polyArr.Count, Allocator.Temp);

        for (int i = 0; i < polyArr.Count; i++)
        {
            var item = polyArr[i];
            var cloneObj = Instantiate(emptyObj, transform);

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