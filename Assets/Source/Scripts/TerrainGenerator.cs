using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public NoiseData    NoiseData;
    public int          WaterLevel  = 50;

    public ChunkData GenerateChunk(ChunkData data, string seed)
    {
        NoiseData.Seed = seed.GetHashCode();

        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                //  Determines the ground level of the noise.
                int groundLevel = GetSurfaceHeight(data.WorldPos.x + x, data.WorldPos.z + z, data.Height);

                //  Alters voxel type base on current height within the chunk.
                for (int y = 0; y < data.Height; y++)
                {
                    VoxelType voxelType = VoxelType.Dirt;

                    //  Places correct voxel type based on the groundLevel and waterLevel
                    if (y > groundLevel)
                    {
                        if (y < WaterLevel)
                        {
                            voxelType = VoxelType.Water;
                        }
                        else
                        {
                            voxelType = VoxelType.Air;
                        }

                    }
                    else if (y == groundLevel)
                    {
                        voxelType = VoxelType.Grass;
                    }

                    //  Sets the current voxel type of the voxel within the chunk.
                    ChunkFunctions.SetVoxelType(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }

        return data;
    }

    private int GetSurfaceHeight(int x, int z, int chunkHeight)
    {
        float height = NoiseGenerator.PerlinOctave(x, z, NoiseData);
        height = NoiseGenerator.Redistribute(height, NoiseData);
        return NoiseGenerator.RemapToChunkHeight(height, 0, chunkHeight);
    }
}
