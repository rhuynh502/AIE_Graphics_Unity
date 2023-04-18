using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "NewNoiseSettings", menuName = "Noise Settings", order = 0)]

public class NoiseSettings : ScriptableObject
{
    public int Seed => seed.GetHashCode();

    public string seed;

    public float scale = 1.5f;
    public int octaves = 8;

    public float persistence = 0.8f;
    public float lacunarity = 0.25f;

    public float2 offset;
}
