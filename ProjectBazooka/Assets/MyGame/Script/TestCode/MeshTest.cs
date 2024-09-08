using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Assign your sprite renderer in the Inspector
    public GameObject meshRenderer;
    public Material Material;

    void Start()
    {
        CreateMeshFromSprite();
    }
    
    private Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0,sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i),0);

        return mesh;
    }

    void CreateMeshFromSprite()
    {
        // Extract sprite vertices
        Vector2[] spriteVertices = spriteRenderer.sprite.vertices;

        // Convert to 3D vertices (assuming Z = 0)
        Vector3[] meshVertices = new Vector3[spriteVertices.Length];
        for (int i = 0; i < spriteVertices.Length; i++)
        {
            meshVertices[i] = new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0);
        }

        // Define triangles for a square (two triangles to form a quad)
        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        // Create a mesh
        var mesh = SpriteToMesh(spriteRenderer.sprite);

        // Attach the mesh to a GameObject with MeshFilter
        MeshFilter meshFilter = meshRenderer.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Optional: Add a MeshRenderer and assign a material
        MeshRenderer meshRen = meshRenderer.AddComponent<MeshRenderer>();
        meshRen.material = Material;
    }
}
