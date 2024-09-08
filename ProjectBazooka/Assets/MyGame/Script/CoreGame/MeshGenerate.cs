using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace MyGame.Script.CoreGame
{
    public abstract class MeshGenerate : MonoBehaviour
    {
        [BurstCompile]
        struct MeshExtrudeJob : IJob
        {
            [Unity.Collections.ReadOnly] public NativeArray<Vector3> vertices;
            [Unity.Collections.ReadOnly] public NativeArray<int> triangles;
            public float depth;
            public NativeList<Vector3> extrudedVertices;
            public NativeList<int> extrudedTriangles;

            public void Execute()
            {
                int vertexCount = vertices.Length;
                int triangleCount = triangles.Length;

                // Map to store index mapping from original to extruded vertex
                NativeArray<int> indexMap = new NativeArray<int>(vertexCount, Allocator.Temp);

                // Extrude vertices
                for (int i = 0; i < vertexCount; i++)
                {
                    extrudedVertices.Add(vertices[i]);
                    extrudedVertices.Add(vertices[i] + Vector3.forward * depth);

                    indexMap[i] = i * 2; // Map original vertex index to the extruded index
                }

                // Extrude triangles
                for (int i = 0; i < triangleCount; i += 3)
                {
                    int v0 = triangles[i];
                    int v1 = triangles[i + 1];
                    int v2 = triangles[i + 2];

                    // Front face
                    extrudedTriangles.Add(indexMap[v0]);
                    extrudedTriangles.Add(indexMap[v1]);
                    extrudedTriangles.Add(indexMap[v2]);

                    // Back face
                    extrudedTriangles.Add(indexMap[v2] + 1);
                    extrudedTriangles.Add(indexMap[v1] + 1);
                    extrudedTriangles.Add(indexMap[v0] + 1);
                }

                // Side faces
                for (int i = 0; i < vertexCount; i++)
                {
                    int next = (i + 1) % vertexCount;

                    // Side face triangles
                    extrudedTriangles.Add(indexMap[i]);
                    extrudedTriangles.Add(indexMap[next]);
                    extrudedTriangles.Add(indexMap[i] + 1);

                    extrudedTriangles.Add(indexMap[next]);
                    extrudedTriangles.Add(indexMap[next] + 1);
                    extrudedTriangles.Add(indexMap[i] + 1);
                }

                // Dispose indexMap after use
                indexMap.Dispose();
            }
        }
        
        public Mesh MeshExtrude(Mesh mesh, float depth)
        {
            NativeArray<Vector3> vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);
            NativeArray<int> triangles = new NativeArray<int>(mesh.triangles, Allocator.TempJob);
            NativeList<Vector3> extrudedVertices = new NativeList<Vector3>(Allocator.TempJob);
            NativeList<int> extrudedTriangles = new NativeList<int>(Allocator.TempJob);

            MeshExtrudeJob job = new MeshExtrudeJob
            {
                vertices = vertices,
                triangles = triangles,
                depth = depth,
                extrudedVertices = extrudedVertices,
                extrudedTriangles = extrudedTriangles
            };

            JobHandle handle = job.Schedule();
            handle.Complete();

            Mesh extrudedMesh = new Mesh
            {
                vertices = extrudedVertices.ToArray(),
                triangles = extrudedTriangles.ToArray()
            };
            
            extrudedMesh.RecalculateNormals();
            extrudedMesh.RecalculateBounds();

            vertices.Dispose();
            triangles.Dispose();
            extrudedVertices.Dispose();
            extrudedTriangles.Dispose();

            return extrudedMesh;
        }
    }
}