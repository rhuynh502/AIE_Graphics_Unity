
using Unity.Mathematics;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh Generate(int _density, float[,] _noiseMap, float _spacing, float _heightScalar = 10f)
    {
        Mesh mesh = new()
        {
            name = "Procedural Mesh"
        };

        Vector3[] verts = new Vector3[_density * _density];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        int[] triangles = new int[(_density - 1) * (_density - 1) * 6];

        int triangleIndex = 0;

        for (int x = 0; x < _density; x++)
        {
            for (int y = 0; y < _density; y++)
            {
                int index = x * _density + y;

                float3 position = new()
                {
                    x = x * _spacing,
                    y = _noiseMap[x, y] * _heightScalar,
                    z = y * _spacing
                };

                CreateVertex(ref verts, ref uvs, ref normals, position, _density, index);

                if (x < _density - 1 && y < _density - 1)
                    AddTriangles(ref triangles, ref triangleIndex, index, _density);
            }
        }

        Finalise(ref mesh, verts, uvs, triangles, normals);

        return mesh;
    }

    private static void AddTriangles(ref int[] _tris, ref int _triIndex, int _currentIndex, int _density)
    {
        _tris[_triIndex++] = _currentIndex + 0;
        _tris[_triIndex++] = _currentIndex + _density + 1;
        _tris[_triIndex++] = _currentIndex + _density;

        _tris[_triIndex++] = _currentIndex + 0;
        _tris[_triIndex++] = _currentIndex + 1;
        _tris[_triIndex++] = _currentIndex + _density + 1;
    }

    private static void CreateVertex(ref Vector3[] _verts, ref Vector2[] _uvs, ref Vector3[] _normals, 
        float3 _position, int _density, int _index)
    {
        _verts[_index] = _position;

        _uvs[_index] = new Vector2
        {
            x = _position.x / _density,
            y = _position.z / _density
        };

        _normals[_index] = Vector3.up;
    }

    private static void Finalise(ref Mesh _mesh, Vector3[] _verts, Vector2[] _uvs, int[] _tris, Vector3[] _normals)
    {
        _mesh.vertices = _verts;
        _mesh.uv = _uvs;
        _mesh.normals = _normals;
        _mesh.triangles = _tris;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }
}
