using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public NoiseData TreeNoiseData;
    public DomainWarping DomainWarping;

    public TreeData GenerateTrees(ChunkData data)
    {
        TreeNoiseData.GetSeed(data.World.Seed);

        TreeData treeData = new TreeData();

        float[,] noiseData = GenerateTreeNoise(data);

        treeData.Trees = NoiseGenerator.LocalMaxima(noiseData, data.WorldPos.x, data.WorldPos.z);

        return treeData;
    }

    private float[,] GenerateTreeNoise(ChunkData data)
    {
        float[,] noiseValues = new float[data.Width, data.Width];

        int xPos = 0;
        int zPos = 0;

        for (int x = data.WorldPos.x; x < data.WorldPos.x + data.Width; x++)
        {
            for (int z = data.WorldPos.z; z < data.WorldPos.z + data.Width; z++)
            {
                noiseValues[xPos, zPos] = DomainWarping.GenerateDomainWarp(x, z, TreeNoiseData);
                zPos++;
            }

            xPos++;
            zPos = 0;
        }
        return noiseValues;
    }
}
