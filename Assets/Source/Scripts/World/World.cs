//  World.cs - Represents the game world, including world data and functions to generate and handle chunks and voxels.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class World : MonoBehaviour
{
    //  VARIABLES:
    public GameController       GameController;
    public GameObject           Player;
    public TerrainGenerator     TerrainGenerator;
    public List<NoiseData>      AllNoiseData;
    public Slider               SeedSlider;
    public GameObject           ChunkObject;
    public WorldData            WorldData;

    [Range(0, 100000)]
    public int                  Seed;

    [Range(4, 32)]
    public int                  ChunkRenderDist = 8;

    public int                  ChunkWidth      = 16;
    public int                  ChunkHeight     = 100;
    public int                  WaterLevel      = 10;
    public float                Gravity         = -20.0f;

    public Dictionary<Vector3Int, ChunkData>        ChunkDataList   = new Dictionary<Vector3Int, ChunkData>();
    public Dictionary<Vector3Int, ChunkRenderer>    Chunks          = new Dictionary<Vector3Int, ChunkRenderer>();

    //  FUNCTIONS:
    //  Generates world when script is loaded.
    public void Awake()
    {
        WorldData.ChunkDataToRemove = new List<Vector3Int>();
        WorldData.ChunksToRemove = new List<Vector3Int>();
        SeedSlider.value = Seed;
        GenerateWorld();
    }

    //  Generates a number of chunks and renders each of the chunks on screen.
    public void GenerateWorld()
    {
        TerrainGenerator.GetBiomeCenters(Player.transform.position, ChunkRenderDist, ChunkWidth);

        foreach (Vector3Int data in WorldData.ChunkDataToRemove)
        {
            ChunkDataList.Remove(data);
        }

        foreach (Vector3Int chunk in WorldData.ChunksToRemove)
        {
            Destroy(Chunks[chunk].gameObject);
            Chunks.Remove(chunk);
        }

        ChangeWorldSeed();

        WorldData = GetWorldData(Vector3Int.FloorToInt(Vector3Int.RoundToInt(Player.transform.position)));

        foreach (Vector3Int chunkPos in WorldData.ChunkDataToCreate)
        {
            ChunkData chunkData = new ChunkData(this, chunkPos, ChunkWidth, ChunkHeight);
            ChunkData terrain = TerrainGenerator.GenerateChunk(chunkData);
            ChunkDataList.Add(chunkPos, terrain);
        }

        foreach (ChunkData chunkPos in ChunkDataList.Values)
        {
            GenerateLeaves(chunkPos);
        }

        //  Generates the chunk mesh and collider, as well as renders the new chunk.
        foreach (Vector3Int chunkPos in WorldData.ChunksToCreate)
        {
            ChunkData data = ChunkDataList[chunkPos];

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

    internal void SetVoxelType(Vector3Int pos, VoxelType newVoxelType)
    {
        ChunkData chunkData = GetChunkData(pos);

        if (chunkData != null)
        {
            Vector3Int voxelPos = ChunkFunctions.GetVoxelChunkPos(chunkData, pos);
            ChunkFunctions.SetVoxelType(chunkData, voxelPos, newVoxelType);
        }
    }

    private ChunkData GetChunkData(Vector3Int pos)
    {
        Vector3Int chunkPos = ChunkFunctions.GetChunkPosFromVoxel(this, pos.x, pos.y, pos.z);

        ChunkData chunkData = null;

        ChunkDataList.TryGetValue(chunkPos, out chunkData);

        return chunkData;
    }

    private void GenerateLeaves(ChunkData chunk)
    {
        foreach(Vector3Int leaf in chunk.TreeData.Leaves)
        {
            ChunkFunctions.SetVoxelType(chunk, leaf, VoxelType.Leaves);
        }
    }

    public void LoadNewChunks()
    {
        GenerateWorld();
        GameController.CheckIfPlayerChunkChanged();
    }

    private WorldData GetWorldData(Vector3Int pos)
    {
        List<Vector3Int> DataInRange = GetDataInRange(pos);
        List<Vector3Int> ChunksInRange = GetChunksInRange(pos);

        List<Vector3Int> NewData = GetAllData(DataInRange, pos);
        List<Vector3Int> NewChunks = GetAllRenderers(ChunksInRange, pos);

        List<Vector3Int> DataToRemove = GetDataOutOfRange(DataInRange, pos);
        List<Vector3Int> ChunksToRemove = GetChunksOutOfRange(ChunksInRange, pos);

        WorldData data = new WorldData
        {
            ChunkDataToCreate = NewData,
            ChunkDataToRemove = DataToRemove,
            ChunksToCreate = NewChunks,
            ChunksToRemove = ChunksToRemove
        };

        return data;
    }

    private List<Vector3Int> GetDataInRange(Vector3Int pos)
    {
        int startX = pos.x - (ChunkRenderDist + 1) * ChunkWidth;
        int startZ = pos.z - (ChunkRenderDist + 1) * ChunkWidth;

        int endX = pos.x + (ChunkRenderDist + 1) * ChunkWidth;
        int endZ = pos.z + (ChunkRenderDist + 1) * ChunkWidth;

        List<Vector3Int> chunkData = new List<Vector3Int>();

        for (int x = startX; x <= endX; x += ChunkWidth)
        {
            for (int z = startZ; z <= endZ; z += ChunkWidth)
            {
                Vector3Int chunkPos = GetChunkPosFromVoxelPos(new Vector3Int(x, 0, z));
                chunkData.Add(chunkPos);
            }
        }

        return chunkData;
    }

    private List<Vector3Int> GetAllData(List<Vector3Int> dataInRange, Vector3Int pos)
    {
        return dataInRange
            .Where(chunk => ChunkDataList.ContainsKey(chunk) == false)
            .OrderBy(chunk => Vector3.Distance(pos, chunk))
            .ToList();
    }

    private List<Vector3Int> GetDataOutOfRange(List<Vector3Int> dataInRange, Vector3Int pos)
    {
        return ChunkDataList.Keys
            .Where(data => dataInRange.Contains(data) == false)
            .ToList();
    }

    private List<Vector3Int> GetChunksInRange(Vector3Int pos)
    {
        int startX = pos.x - ChunkRenderDist * ChunkWidth;
        int startZ = pos.z - ChunkRenderDist * ChunkWidth;

        int endX = pos.x + ChunkRenderDist * ChunkWidth;
        int endZ = pos.z + ChunkRenderDist * ChunkWidth;

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
            .Where(chunk => Chunks.ContainsKey(chunk) == false)
            .OrderBy(chunk => Vector3.Distance(pos, chunk))
            .ToList();
    }

    private List<Vector3Int> GetChunksOutOfRange(List<Vector3Int> chunksInRange, Vector3Int pos)
    {
        List<Vector3Int> chunksToRemove = new List<Vector3Int>();

        foreach(Vector3Int chunk in Chunks.Keys
            .Where(renderer => chunksInRange.Contains(renderer) == false))
        {
            if (Chunks.ContainsKey(chunk))
            {
                chunksToRemove.Add(chunk);
            }
        }

        return chunksToRemove;
    }

    public void ChangeWorldSeed()
    {
        foreach (NoiseData noiseData in AllNoiseData)
        {
            noiseData.GetSeed(Mathf.RoundToInt(SeedSlider.value));
        }
    }

    public Vector3Int GetChunkPosFromVoxelPos(Vector3Int pos)
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

    //  Only enabled if using unity editor.
#if UNITY_EDITOR
    //  Draws outlines of selected chunks in the scene window if gizmos are enabled.
    private void OnDrawGizmos()
    {
        //  Ensure application is running and that there is chunk data before eneabling selection.
        if (Application.isPlaying && Player != null)
        {
            Gizmos.color = Color.green;
            Vector3 cubeCenter = GameController.PlayerChunkCenter;
            float cubeSize = 2.0f * ((ChunkWidth) * (ChunkRenderDist)) + ChunkWidth;
            Gizmos.DrawWireCube(cubeCenter, new Vector3(cubeSize, 1.0f, cubeSize));

            Gizmos.color = Color.blue;
            cubeSize += ChunkWidth * 2.0f;
            Gizmos.DrawWireCube(cubeCenter, new Vector3(cubeSize, 1.0f, cubeSize));
        }
    }
#endif
}