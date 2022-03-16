//  World.cs - Represents the game world, including world data and functions to generate and handle chunks and voxels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class World : MonoBehaviour
{
    //  VARIABLES:
    public GameObject                       Player;
    public TerrainGenerator                 TerrainGenerator;
    public InputField                       SeedInputField;
    public GameObject                       ChunkObject;

    Dictionary<Vector3Int, ChunkRenderer>   Chunks          = new Dictionary<Vector3Int, ChunkRenderer>();
    Dictionary<Vector3Int, ChunkData>       ChunkDataList   = new Dictionary<Vector3Int, ChunkData>();

    public string                           Seed;
    [Range(8, 32)]
    public int                              NumChunks       = 8;
    public int                              ChunkWidth      = 16;
    public int                              ChunkHeight     = 100;
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

        SetSeed();

        //  Creates new chunks by generating the voxels within each chunk.
        for (int x = 0; x < NumChunks; x++)
        {
            for (int z = 0; z < NumChunks; z++)
            {
                //  Generate voxels for this chunk and add it to the list.
                ChunkData data = new ChunkData(this, new Vector3Int(x * ChunkWidth, 0, z * ChunkWidth), ChunkWidth, ChunkHeight);
                ChunkData terrain = TerrainGenerator.GenerateChunk(data, Seed);
                ChunkDataList.Add(terrain.WorldPos, terrain);
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

    public void SetSeed()
    {
        Seed = SeedInputField.text;
    }

    //  Handles generation of the voxels for each chunk within the game, using Perlin noise to generate shapes of the landmass.
    private void GenerateVoxels(ChunkData data)
    {

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