using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public NoiseData NoiseData;
    public LayerHandler InitialLayer;
    public List<LayerHandler> MiscHandlers;
<<<<<<< HEAD:Assets/Source/Scripts/Terrain/BiomeGenerator.cs
    public bool UseDomainWarping;
=======
>>>>>>> parent of 417f71f (still working with domain warp):Assets/Source/Scripts/BiomeGenerator.cs

    public  ChunkData GetChunkData(ChunkData data, int x, int z)
    {
        //  Determines the ground level of the noise.
        int groundLevel = GetSurfaceHeight(data.WorldPos.x + x, data.WorldPos.z + z, data.Height);

        //  Alters voxel type base on current height within the chunk.
        for (int y = 0; y < data.Height; y++)
        {
            InitialLayer.HandleLayer(data, new Vector3Int(x, y, z), groundLevel);
        }

        foreach (LayerHandler handler in MiscHandlers)
        {
            handler.HandleLayer(data, new Vector3Int(x, data.WorldPos.y, z), groundLevel);
        }

        return data;
    }

    private int GetSurfaceHeight(int x, int z, int chunkHeight)
    {
<<<<<<< HEAD:Assets/Source/Scripts/Terrain/BiomeGenerator.cs
        float height = 0;
        if (UseDomainWarping)
        {
             height = DomainWarping.GenerateDomainWarp(x, z, NoiseData);
        }
        else
        {
             height = NoiseGenerator.PerlinOctave(x, z, NoiseData);
        }

=======
        float height = NoiseGenerator.PerlinOctave(x, z, NoiseData);
>>>>>>> parent of 417f71f (still working with domain warp):Assets/Source/Scripts/BiomeGenerator.cs
        height = NoiseGenerator.Redistribute(height, NoiseData);
        return NoiseGenerator.RemapToChunkHeight(height, 0, chunkHeight);
    }
}
