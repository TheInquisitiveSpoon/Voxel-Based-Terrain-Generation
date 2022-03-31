//  DomainWarping.cs - Warps generated noise, creating interesting stretches and twists in the values.
//  Some functions used from the following source:
//  Accessible at:  http://www.nolithius.com/articles/world-generation/world-generation-techniques-domain-warping

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class DomainWarping : MonoBehaviour
{
    //  REFERENCES:
    public NoiseData    XNoise;
    public NoiseData    ZNoise;

    //  VARIABLES:
    public int          AmplitudeX;
    public int          AmplitudeZ;

    //  Gets the offset x, z coordinate then adds them to the current x, z coordinate
    //  performing octave perlin using the new position.
    public float GenerateDomainWarp(int x, int z, NoiseData noiseData)
    {
        Vector2 warp = GetOffSet(x, z);
        return NoiseGenerator.PerlinOctave(x + warp.x, z + warp.y, noiseData);
    }

    //  Generates the noise value for the x, z coordinates, then warps them by multiplying
    //  by the amplitude.
    public Vector2 GetOffSet(int x, int z)
    {
        return new Vector2
        {
            x = NoiseGenerator.PerlinOctave(x, z, XNoise) * AmplitudeX,
            y = NoiseGenerator.PerlinOctave(x, z, ZNoise) * AmplitudeZ
        };
    }

    //  Rounds the warp value to an int.
    public Vector2Int GetIntOffset(int x, int z)
    {
        return Vector2Int.RoundToInt(GetOffSet(x, z));
    }
}