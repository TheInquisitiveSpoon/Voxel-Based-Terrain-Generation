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

    public static List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int( 0, 1), //N
        new Vector2Int( 1, 1), //NE
        new Vector2Int( 1, 0), //E
        new Vector2Int(-1, 1), //SE
        new Vector2Int(-1, 0), //S
        new Vector2Int(-1,-1), //SW
        new Vector2Int( 0,-1), //W
        new Vector2Int( 1,-1)  //NW
    };

    public static List<Vector2Int> LocalMaxima(float[,] noiseData, int xPos, int zPos)
    {
        List<Vector2Int> maximaList = new List<Vector2Int>();

        for (int x = 0; x < noiseData.GetLength(0); x++)
        {
            for (int y = 0; y < noiseData.GetLength(1); y++)
            {
                float value = noiseData[x, y];

                if (GetNeighbouringNoise(noiseData, x, y, (neighbourNoise) => neighbourNoise < value))
                {
                    maximaList.Add(new Vector2Int(xPos + x, zPos + y));
                }

            }
        }
        return maximaList;
    }

    private static bool GetNeighbouringNoise(float[,] noiseData, int x, int y, Func<float, bool> success)
    {
        foreach (var dir in directions)
        {
            var newPost = new Vector2Int(x + dir.x, y + dir.y);

            if (newPost.x < 0 || newPost.x >= noiseData.GetLength(0) || newPost.y < 0 || newPost.y >= noiseData.GetLength(1))
            {
                continue;
            }

            if (success(noiseData[x + dir.x, y + dir.y]) == false)
            {
                return false;
            }
        }
        return true;
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
