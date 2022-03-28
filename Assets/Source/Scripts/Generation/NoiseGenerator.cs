using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float PerlinOctave(float x, float z, NoiseData data)
    {
        x *= data.Magnitude;
        z *= data.Magnitude;
        x += data.Magnitude;
        z += data.Magnitude;

        float finalVal = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeCount = 0;  // Used for normalizing result to 0.0 - 1.0 range

        for (int i = 0; i < data.Octaves; i++)
        {
            finalVal += Mathf.PerlinNoise(((data.Seed) + data.Offset.x + x) * frequency, ((data.Seed) + data.Offset.y + z) * frequency) * amplitude;

            amplitudeCount += amplitude;

            amplitude *= data.Persistence;
            frequency *= 2;
        }

        return finalVal / amplitudeCount;
    }

    public static float Redistribute(float noise, NoiseData data)
    {
        return Mathf.Pow(noise * data.Modifier, data.PlateauExponent);
    }

    public static int RemapToChunkHeight(float value, float min, float max)
    {
        return (int)((min + value - 0) * (max - min) / (1 - 0));
    }
}
