using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainWarping : MonoBehaviour
{
    public NoiseData XNoise;
    public NoiseData ZNoise;

    public int AmplitudeX;
    public int AmplitudeZ;

    public float GenerateDomainWarp(int x, int z, NoiseData noiseData)
    {
        Vector2 warp = GetOffSet(x, z);
        return NoiseGenerator.PerlinOctave(x + warp.x, z + warp.y, noiseData);
    }

    public Vector2 GetOffSet(int x, int z)
    {
        return new Vector2
        {
            x = NoiseGenerator.PerlinOctave(x, z, XNoise) * AmplitudeX,
            y = NoiseGenerator.PerlinOctave(x, z, ZNoise) * AmplitudeZ
        };
    }

    public Vector2Int GetIntOffset(int x, int z)
    {
        return Vector2Int.RoundToInt(GetOffSet(x, z));
    }
}