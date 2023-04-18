using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GeneratePlane : MonoBehaviour
{
    [SerializeField] private int density = 64;
    [SerializeField] private float spacing = .25f;

    private MeshFilter filter;
    private new MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();

        filter.mesh = CreateMesh();
        renderer.material = CreateMaterial();
    }

    private Material CreateMaterial()
    {
        Material material = new Material(Shader.Find("Unlit/Texture"));

        material.SetTexture("_MainTex", CreateTexture());

        return material;
    }

    private Texture2D CreateTexture()
    {
        Texture2D texture = new(density, density, TextureFormat.RGBA64, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        for(int x = 0; x < density; x++)
        {
            for(int y = 0; y < density; y++)
            {
                float n = noise.cnoise(new float2(x * spacing, y * spacing));

                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, NoiseFunctions.Remap(n, -1f, 1f, 0f, 1f)));
            }
        }

        texture.Apply();

        return texture;
    }

    private Mesh CreateMesh()
    {
        Vector3[] verts = new Vector3[density * density];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int[] triangles = new int[(density - 1) * (density - 1) * 6];

        int triangleIndex = 0;

        for (int i = 0; i < density; i++)
        {
            for (int j = 0; j < density; j++)
            {
                int index = i * density + j;

                verts[index] = new Vector3
                {
                    x = i * spacing,
                    y = 0,
                    z = j * spacing
                };

                uvs[index] = new Vector2
                {
                    x = i / (float)density,
                    y = j / (float)density
                };

                normals[index] = Vector3.up;

                if(i < density - 1 && j < density - 1)
                {
                    triangles[triangleIndex++] = index + 0;
                    triangles[triangleIndex++] = index + 1 + density;
                    triangles[triangleIndex++] = index + density;

                    triangles[triangleIndex++] = index + 0;
                    triangles[triangleIndex++] = index + 1;
                    triangles[triangleIndex++] = index + 1 + density;

                }

            }
        }

        Mesh mesh = new()
        {
            vertices = verts,
            normals = normals,
            uv = uvs,
            triangles = triangles
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
