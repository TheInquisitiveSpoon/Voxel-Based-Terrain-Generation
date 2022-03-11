//  World.cs - Represents the game world, including world data and functions to generate and handle chunks and voxels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class World : MonoBehaviour
{
    //  VARIABLES:
    public GameObject                       ChunkObject;

    Dictionary<Vector3Int, ChunkRenderer>   Chunks          = new Dictionary<Vector3Int, ChunkRenderer>();
    Dictionary<Vector3Int, ChunkData>       ChunkDataList   = new Dictionary<Vector3Int, ChunkData>();

    public int                              NumChunks       = 6;
    public int                              ChunkWidth      = 16;
    public int                              ChunkHeight     = 100;
    public int                              WaterLevel      = 50;
    public float                            Noise           = 0.03f;
    public float                            Gravity         = -20.0f;

    //  FUNCTIONS:
    //  Generates world when script is loaded.
    public void Awake()
    {
        GenerateWorld();
    }

    //  Generates a number of chunks and renders each of the chunks on screen.
    public void GenerateWorld()
    {
        ChunkDataList.Clear();

        //  Destroys existing chunk game objects
        foreach (ChunkRenderer chunk in Chunks.Values)
        {
            Destroy(chunk.gameObject);
        }

        Chunks.Clear();

        //  Creates new chunks by generating the voxels within each chunk.
        for (int x = 0; x < NumChunks; x++)
        {
            for (int z = 0; z < NumChunks; z++)
            {
                //  Generate voxels for this chunk and add it to the list.
                ChunkData data = new ChunkData(this, new Vector3Int(x * ChunkWidth, 0, z * ChunkWidth), ChunkWidth, ChunkHeight);
                GenerateVoxels(data);
                ChunkDataList.Add(data.WorldPos, data);
            }
        }

        //  Generates the chunk mesh and collider, as well as renders the new chunk.
        foreach (ChunkData data in ChunkDataList.Values)
        {
            //  Gets mesh data.
            MeshHandler meshData = ChunkFunctions.GetMeshData(data);

            //  Creates a new gameobject for the chunk.
            GameObject chunkObject = Instantiate(ChunkObject, data.WorldPos, Quaternion.identity);

            //  Renders the new chunk using the data.  
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkRenderer.SetChunkData(data);
            chunkRenderer.UpdateChunk(meshData);

            //  Adds chunk to the list.
            Chunks.Add(data.WorldPos, chunkRenderer);
        }
    }

    //  Handles generation of the voxels for each chunk within the game, using Perlin noise to generate shapes of the landmass.
    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                //  Determines the 2D noise value for the current voxel.
                float noise = Mathf.PerlinNoise((data.WorldPos.x + x) * Noise, (data.WorldPos.z + z) * Noise);
                
                //  Determines the ground level of the noise.
                int groundLevel = Mathf.RoundToInt(noise * ChunkHeight);

                //  Alters voxel type base on current height within the chunk.
                for (int y = 0; y < ChunkHeight; y++)
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
    }

    //  Returns the voxel type of the voxel at the specified chunk position.
    internal VoxelType GetVoxelTypeFromChunkPos(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = ChunkFunctions.GetChunkPosFromVoxel(this, x, y, z);

        //  Attempts to find the current chunk and returns it to the tempChunk, returns Nothing if null.
        ChunkData tempChunk = null;
        ChunkDataList.TryGetValue(pos, out tempChunk);
        if (tempChunk == null) { return VoxelType.Nothing; }

        //  Gets the chunkPos of the voxel, then returns the voxel type using the position.
        Vector3Int chunkPos = ChunkFunctions.GetVoxelChunkPos(tempChunk, new Vector3Int(x, y, z));
        return ChunkFunctions.GetVoxelTypeFromPos(tempChunk, chunkPos.x, chunkPos.y, chunkPos.z);
    }
}