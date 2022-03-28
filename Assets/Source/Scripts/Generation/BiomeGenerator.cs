using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    public NoiseData NoiseData;
    public DomainWarping DomainWarping;
    public LayerHandler InitialLayer;
    public List<LayerHandler> MiscHandlers;
    public TreeGenerator TreeGenerator;

    public  ChunkData GetChunkData(ChunkData data, int x, int z)
    {
        //  Determines the ground level of the noise.
        int groundLevel = GetSurfaceHeight(data.WorldPos.x + x, data.WorldPos.z + z, data.Height);

        //  Alters voxel type base on current height within the chunk.
        for (int y = 0; y < data.Height; y++)
        {
            if (y == 0)
            {
                ChunkFunctions.SetVoxelType(data, new Vector3Int(x, y, z), VoxelType.Bedrock);
            }

            InitialLayer.HandleLayer(data, new Vector3Int(x, y, z), groundLevel);
        }

        foreach (LayerHandler handler in MiscHandlers)
        {
            handler.HandleLayer(data, new Vector3Int(x, data.WorldPos.y, z), groundLevel);
        }

        return data;
    }

    public TreeData GenerateTreeData(ChunkData data)
    {
        if (TreeGenerator == null) { return new TreeData(); }

        return TreeGenerator.GenerateTrees(data);
    }

    private int GetSurfaceHeight(int x, int z, int chunkHeight)
    {
        float height = DomainWarping.GenerateDomainWarp(x, z, NoiseData);
        height = NoiseGenerator.Redistribute(height, NoiseData);
        return NoiseGenerator.RemapToChunkHeight(height, 0, chunkHeight);
    }
}
