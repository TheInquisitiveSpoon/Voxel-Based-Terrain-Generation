using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator BiomeGenerator;

    public ChunkData GenerateChunk(ChunkData data)
    {
        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                data = BiomeGenerator.GetChunkData(data, x, z);
            }
        }

        return data;
    }
}
