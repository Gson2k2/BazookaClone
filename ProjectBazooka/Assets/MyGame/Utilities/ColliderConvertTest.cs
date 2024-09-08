using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Utilities
{
    public class ColliderConvertTest : MonoBehaviour
    {
        public float depth = 1f; // Depth of the 3D collider
        public List<PolygonCollider2D> polyCollider;

        private void OnValidate()
        {
            foreach (var item in transform.GetComponentsInChildren<PolygonCollider2D>())
            {
                polyCollider.Add(item);
            }
        }

        void Start()
        {
            if (polyCollider != null)
            {
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                foreach (var item in polyCollider)
                {
                    meshCollider.sharedMesh = CreateMeshFromPolygonCollider(item);
                }
            }
        }

        Mesh CreateMeshFromPolygonCollider(PolygonCollider2D polyCollider)
        {
            Mesh mesh = new Mesh();
            Vector2[] points = polyCollider.points;
            Vector3[] vertices = new Vector3[points.Length * 2];
            int[] triangles = new int[(points.Length - 1) * 6];

            for (int i = 0; i < points.Length; i++)
            {
                vertices[i] = new Vector3(points[i].x, points[i].y, 0);
                vertices[i + points.Length] = new Vector3(points[i].x, points[i].y, depth);
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                int j = i * 6;
                triangles[j] = i;
                triangles[j + 1] = i + 1;
                triangles[j + 2] = i + points.Length;

                triangles[j + 3] = i + 1;
                triangles[j + 4] = i + points.Length + 1;
                triangles[j + 5] = i + points.Length;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
