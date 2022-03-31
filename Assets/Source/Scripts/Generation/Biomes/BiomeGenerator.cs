//  BiomeGenerator.cs - Generates different biomes using biome specific information and layer handlers to generate noise and place voxels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    //  REFERENCES:
    public World                World;
    public NoiseData            NoiseData;
    public DomainWarping        DomainWarping;
    public LayerHandler         InitialLayer;
    public List<LayerHandler>   MiscHandlers;

    //  VARIABLES:
    public BiomeType BiomeType;

    [Range(0.0f, 1.0f)]
    public float MinTemperature;

    [Range(0.0f, 1.0f)]
    public float MaxTemperature;

    [Range(0.0f, 1.0f)]
    public float MinPrecipitation;

    [Range(0.0f, 1.0f)]
    public float MaxPrecipitation;

    //  Gets the data for the chunk by generating surface height noise if it has not been
    //  generated already, then loops through layer handlers to generate specific layers
    //  of the biome by setting each voxel to the correct type.
    public ChunkData GetChunkData(ChunkData data, int x, int z, int? biomeGroundLevel)
    {
        //  Noise height value for the current voxel.
        int groundLevel;

        //  Calculates noise height for the voxel if the value has not been provided already.
        if (biomeGroundLevel.HasValue == false)
        {
            groundLevel = GetSurfaceHeight(data.WorldPos.x + x, data.WorldPos.z + z, data.Height);
        }
        else
        {
            groundLevel = biomeGroundLevel.Value;
        }

        //  Loops through each voxel in the Y Axis and passes it to the layer handlers to determine
        //  what block to place, if the layer does not process the block then it passes it to the
        //  next handler.
        for (int y = 0; y < data.Height; y++)
        {
            //  Places bedrock at the bottom of the chunk.
            if (y == 0)
            {
                ChunkFunctions.SetVoxelType(data, new Vector3Int(x, y, z), VoxelType.Bedrock);
            }

            //  Passes the voxel position to the first layer handler.
            InitialLayer.HandleLayer(data, new Vector3Int(x, y, z), groundLevel);
        }

        //  Unique layer handlers that may process all of the voxels in the Y axis.
        foreach (LayerHandler handler in MiscHandlers)
        {
            handler.HandleLayer(data, new Vector3Int(x, data.WorldPos.y, z), groundLevel);
        }

        return data;
    }

    //  Generates the surface height value of the specific voxel in the x and z axis,
    //  uses domain warping and octave perlin noise which is remapped to an int based
    //  on the chunk height.
    public int GetSurfaceHeight(int x, int z, int chunkHeight)
    {
        float height    = DomainWarping.GenerateDomainWarp(x, z, NoiseData);
        height          = NoiseGenerator.Redistribute(height, NoiseData);
        return NoiseGenerator.RemapToChunkHeight(height, 0, chunkHeight);
    }
}
