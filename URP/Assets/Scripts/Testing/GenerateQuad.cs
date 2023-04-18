using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class GenerateQuad : MonoBehaviour
{
    private MeshFilter filter;
    private new MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        renderer = GetComponent<MeshRenderer>();

        Vector3[] verts = new Vector3[]
        {
            Vector3.up,
            Vector3.up + Vector3.right,
            Vector3.right,
            Vector3.zero
        };

        int[] tris =
        {
            0, 1, 3,
            1, 2, 3
        };

        Vector3[] normals =
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };

        Vector2[] uvs =
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };

        Color[] colors =
        {
            Color.green,
            Color.red,
            Color.blue,
            Color.yellow
        };

        Mesh mesh = new()
        {
            vertices = verts,
            uv = uvs,
            normals = normals,
            triangles = tris,
            colors = colors
        };

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        filter.mesh = mesh;

        Texture2D texture = new(64, 64, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            alphaIsTransparency = true
        };
        

        bool isGrey = true;
        for(int i = 0; i < texture.width; i++)
        {
            for(int j = 0; j < texture.height; j++)
            {
                if((j * texture.width + i) % 4 == 0)
                {
                    isGrey = !isGrey;
                }

                Color color = isGrey ? Color.gray : Color.white;
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();

        renderer.material.mainTexture = texture;
    }

}
