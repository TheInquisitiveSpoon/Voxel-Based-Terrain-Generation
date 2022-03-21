//  World.cs - Represents the game world, including world data and functions to generate and handle chunks and voxels.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class World : MonoBehaviour
{
    //  VARIABLES:
    public GameObject                       Player;
    public TerrainGenerator                 TerrainGenerator;
    public Slider                           SeedSlider;
    public NoiseData                        NoiseData;
    public GameObject                       ChunkObject;

    [Range(0, 500000)]
    public int                              Seed;

    [Range(8, 32)]
    public int                              NumChunks       = 8;

    public int                              ChunkWidth      = 16;
    public int                              ChunkHeight     = 100;
    public int                              WaterLevel      = 10;
    public float                            Gravity         = -20.0f;

    public WorldData WorldData = new WorldData();
    public Dictionary<Vector3Int, ChunkData> ChunkDataList = new Dictionary<Vector3Int, ChunkData>();
    public Dictionary<Vector3Int, ChunkRenderer> Chunks = new Dictionary<Vector3Int, ChunkRenderer>();

    //  FUNCTIONS:
    //  Generates world when script is loaded.
    public void Awake()
    {
        GenerateWorld();
        WorldData.ChunkDataToCreate = new List<Vector3Int>();
        WorldData.ChunksToCreate = new List<Vector3Int>();
    }

    //  Generates a number of chunks and renders each of the chunks on screen.
    public void GenerateWorld()
    {
        ChunkDataList.Clear();

        ChangeWorldSeed();

        WorldData = GetWorldData(Vector3Int.RoundToInt(Player.transform.position));

        foreach(Vector3Int chunk in WorldData.ChunkDataToCreate)
        {
            ChunkData data = new ChunkData(this, chunk, ChunkWidth, ChunkHeight);
            ChunkData terrain = TerrainGenerator.GenerateChunk(data);
            ChunkDataList.Add(chunk, terrain);
        }


        //  Generates the chunk mesh and collider, as well as renders the new chunk.
        foreach (Vector3Int chunk in WorldData.ChunksToCreate)
        {
            ChunkData data = ChunkDataList[chunk];

            //  Gets mesh data.
            MeshHandler meshData = ChunkFunctions.GetMeshData(data);

            //  Creates a new gameobject for the chunk.
            GameObject chunkObject = Instantiate(ChunkObject, chunk, Quaternion.identity);

            //  Renders the new chunk using the data.  
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkRenderer.SetChunkData(data);
            chunkRenderer.UpdateChunk(meshData);

            //  Adds chunk to the list.
            Chunks.Add(data.WorldPos, chunkRenderer);
        }
    }

    private WorldData GetWorldData(Vector3Int pos)
    {
        List<Vector3Int> DataInRange = GetDataInRange(pos);
        List<Vector3Int> NewData = GetAllData(DataInRange, pos);

        List<Vector3Int> ChunksInRange = GetChunksInRange(pos);
        List<Vector3Int> NewChunks = GetAllRenderers(ChunksInRange, pos);

        WorldData updatedData = new WorldData
        {
            ChunkDataToCreate = NewData,
            ChunksToCreate = NewChunks,
            ChunksToRemove = new List<Vector3Int>(),
            ChunkDataToRemove = new List<Vector3Int>()
        };

        return updatedData;
    }

    private List<Vector3Int> GetDataInRange(Vector3Int pos)
    {
        int startX = pos.x - (NumChunks + 1) * ChunkWidth;
        int startZ = pos.z - (NumChunks + 1) * ChunkWidth;

        int endX = pos.x + (NumChunks + 1) * ChunkWidth;
        int endZ = pos.z + (NumChunks + 1) * ChunkWidth;

        List<Vector3Int> chunks = new List<Vector3Int>();

        for (int x = startX; x <= endX; x += ChunkWidth)
        {
            for (int z = startZ; z <= endZ; z += ChunkWidth)
            {
                Vector3Int chunkPos = GetChunkPosFromVoxelPos(new Vector3Int(x, 0, z));
                chunks.Add(chunkPos);
            }
        }

        return chunks;
    }

    private List<Vector3Int> GetAllData(List<Vector3Int> dataInRange, Vector3Int pos)
    {
        return dataInRange
             .Where(chunkPos => ChunkDataList.ContainsKey(chunkPos) == false)
             .OrderBy(chunkPos => Vector3.Distance(pos, chunkPos))
             .ToList();
    }

    private List<Vector3Int> GetChunksInRange(Vector3Int pos)
    {
        int startX = pos.x - NumChunks * ChunkWidth;
        int startZ = pos.z - NumChunks * ChunkWidth;

        int endX = pos.x + NumChunks * ChunkWidth;
        int endZ = pos.z + NumChunks * ChunkWidth;

        List<Vector3Int> chunks = new List<Vector3Int>();

        for (int x = startX; x <= endX; x += ChunkWidth)
        {
            for (int z = startZ; z <= endZ; z += ChunkWidth)
            {
                Vector3Int chunkPos = GetChunkPosFromVoxelPos(new Vector3Int(x, 0, z));
                chunks.Add(chunkPos);
            }
        }

        return chunks;
    }

    private List<Vector3Int> GetAllRenderers(List<Vector3Int> chunksInRange, Vector3Int pos)
    {
        return chunksInRange
            .Where(chunk => Chunks.ContainsKey(pos) == false)
            .OrderBy(chunk => Vector3.Distance(pos, chunk))
            .ToList();
    }

    public void ChangeWorldSeed()
    {
        NoiseData.Seed = Mathf.RoundToInt(SeedSlider.value);
    }

    private Vector3Int GetChunkPosFromVoxelPos(Vector3Int pos)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(pos.x / (float)ChunkWidth) * ChunkWidth,
            y = Mathf.FloorToInt(pos.y / (float)ChunkHeight) * ChunkHeight,
            z = Mathf.FloorToInt(pos.z / (float)ChunkWidth) * ChunkWidth
        };
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