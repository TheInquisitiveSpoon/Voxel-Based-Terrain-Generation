//  World.cs - Represents the game world, including world data and functions to generate and handle chunks and voxels.
//  Class based on the following sources:
//  Available at:   https://web.archive.org/web/20150214125646/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial-pt-3
//                  https://web.archive.org/web/20150311034426/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial-pt-4
//                  https://web.archive.org/web/20150318012606/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial-pt-7

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class World : MonoBehaviour
{
    //  REFERENCES:
    public GameObject           Player;
    public GameController       GameController;
    public TerrainGenerator     TerrainGenerator;
    public Slider               SeedSlider;
    public Slider               RenderDistSlider;
    public Text                 ChunksText;
    public Text                 VoxelsText;
    public GameObject           ChunkObject;
    public List<NoiseData>      AllNoiseData;

    //  VARIABLES:
    public WorldData                                WorldData;
    public Dictionary<Vector3Int, ChunkData>        ChunkDataList   = new Dictionary<Vector3Int, ChunkData>();
    public Dictionary<Vector3Int, ChunkRenderer>    Chunks          = new Dictionary<Vector3Int, ChunkRenderer>();
    public int                                      ChunkWidth      = 16;
    public int                                      ChunkHeight     = 100;
    public int                                      WaterLevel      = 10;
    public float                                    Gravity         = -20.0f;

    //  RANGE VARIABLES:
    [Range(0, 100000)]
    public int  Seed;

    [Range(4, 32)]
    public int  ChunkRenderDist     = 8;

    //  FUNCTIONS:
    //  Generates world when script is loaded.
    public void Awake()
    {
        WorldData.ChunkDataToRemove = new List<Vector3Int>();
        WorldData.ChunksToRemove = new List<Vector3Int>();
        SeedSlider.value = Seed;
        GenerateWorld();
    }

    //  Generates biomes, removes old chunks, makes new ones, adding world seed, generates chunk data
    //  renders chunks.
    public void GenerateWorld()
    {
        //  Removes chunk data out of range.
        foreach (Vector3Int data in WorldData.ChunkDataToRemove)
        {
            ChunkDataList.Remove(data);
        }

        //  Destroys and removes chunks out of range.
        foreach (Vector3Int chunk in WorldData.ChunksToRemove)
        {
            Destroy(Chunks[chunk].gameObject);
            Chunks.Remove(chunk);
        }

        //  Sets the seed to all noise data.
        ChangeWorldSeed();

        //  Sets the render distance to a new distance.
        ChangeRenderDist();

        //  Gets the biome centers to determine chunk biomes.
        TerrainGenerator.GetBiomeCenters(Player.transform.position, ChunkRenderDist, ChunkWidth);

        //  Gets what chunks and chunk data need to be created and removed.
        WorldData = GetWorldData(Vector3Int.RoundToInt(Player.transform.position));

        //  Creates chunk data for each chunk needed.
        foreach (Vector3Int chunkPos in WorldData.ChunkDataToCreate)
        {
            ChunkData chunkData     = new ChunkData(this, chunkPos, ChunkWidth, ChunkHeight);
            ChunkData terrain       = TerrainGenerator.GenerateChunk(chunkData);

            ChunkDataList.Add(chunkPos, terrain);
        }

        //  Places each leaf in the chunk leaves.
        foreach (ChunkData chunkPos in ChunkDataList.Values)
        {
            GenerateLeaves(chunkPos);
        }

        //  Generates the chunk mesh and collider, as well as renders the new chunk.
        foreach (Vector3Int chunkPos in WorldData.ChunksToCreate)
        {
            ChunkData data          = ChunkDataList[chunkPos];

            //  Gets mesh data.
            MeshHandler meshData    = ChunkFunctions.GetMeshData(data);

            //  Creates a new gameobject for the chunk.
            GameObject chunkObject  = Instantiate(ChunkObject, data.WorldPos, Quaternion.identity);

            //  Renders the new chunk using the data.  
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkRenderer.SetChunkData(data);
            chunkRenderer.UpdateChunk(meshData);

            //  Adds chunk to the list.
            Chunks.Add(data.WorldPos, chunkRenderer);
        }

        ChunksText.text = "Chunks: " + Chunks.Count;
        VoxelsText.text = "Voxels: " + (Chunks.Count * (ChunkWidth * ChunkHeight * ChunkWidth));
    }

    //  Loads new world chunks, and restarts the coroutine to check again after a delay.
    public void LoadNewChunks()
    {
        GenerateWorld();
        GameController.CheckIfPlayerChunkChanged();
    }

    //  Updates world data by getting all the chunks and chunk data in range or out of range.
    private WorldData GetWorldData(Vector3Int pos)
    {
        //  Gets the chunks and chunk data within the render distance.
        List<Vector3Int> DataInRange        = GetDataInRange(pos);
        List<Vector3Int> ChunksInRange      = GetChunksInRange(pos);

        //  Determines what chunks and chunk data need to be generated.
        List<Vector3Int> NewData            = GetAllData(DataInRange, pos);
        List<Vector3Int> NewChunks          = GetAllRenderers(ChunksInRange, pos);

        //  Determines what chunks and chunk data need to be removed.
        List<Vector3Int> DataToRemove       = GetDataOutOfRange(DataInRange, pos);
        List<Vector3Int> ChunksToRemove     = GetChunksOutOfRange(ChunksInRange, pos);

        return new WorldData
        {
            ChunkDataToCreate = NewData,
            ChunkDataToRemove = DataToRemove,
            ChunksToCreate = NewChunks,
            ChunksToRemove = ChunksToRemove
        };
    }

    //  Determines which chunk data is in range of the player.
    //  Distance is increased by one to generate chunk data before trying to render.
    private List<Vector3Int> GetDataInRange(Vector3Int pos)
    {
        //  Initial positions of loop.
        int startX = pos.x - (ChunkRenderDist + 1) * ChunkWidth;
        int startZ = pos.z - (ChunkRenderDist + 1) * ChunkWidth;

        //  End positions of loop.
        int endX = pos.x + (ChunkRenderDist + 1) * ChunkWidth;
        int endZ = pos.z + (ChunkRenderDist + 1) * ChunkWidth;

        List<Vector3Int> chunkData = new List<Vector3Int>();

        //  Iterates through the chunk positions to add the data to the list.
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

    //  Adds chunk data that are in range of the player to the chunk data list,
    //  if they are not already in the list.
    private List<Vector3Int> GetAllData(List<Vector3Int> dataInRange, Vector3Int pos)
    {
        //  Ensure the list is ordered by distance to get nearest chunk data first.
        return dataInRange
            .Where(chunk => ChunkDataList.ContainsKey(chunk) == false)
            .OrderBy(chunk => Vector3.Distance(pos, chunk))
            .ToList();
    }

    //  Gets chunk data out of range of the player to a list.
    private List<Vector3Int> GetDataOutOfRange(List<Vector3Int> dataInRange, Vector3Int pos)
    {
        return ChunkDataList.Keys
            .Where(data => dataInRange.Contains(data) == false)
            .ToList();
    }

    //  Determines which chunks are in range of the player.
    private List<Vector3Int> GetChunksInRange(Vector3Int pos)
    {
        //  Initial positions of loop.
        int startX = pos.x - ChunkRenderDist * ChunkWidth;
        int startZ = pos.z - ChunkRenderDist * ChunkWidth;

        //  End positions of loop.
        int endX = pos.x + ChunkRenderDist * ChunkWidth;
        int endZ = pos.z + ChunkRenderDist * ChunkWidth;

        List<Vector3Int> chunks = new List<Vector3Int>();

        //  Iterates through the chunk positions to add the chunk to the list.
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

    //  Adds chunks that are in range of the player to the chunk list,
    //  if they are not already in the list.
    private List<Vector3Int> GetAllRenderers(List<Vector3Int> chunksInRange, Vector3Int pos)
    {
        //  Ensure the list is ordered by distance to get nearest chunks first.
        return chunksInRange
            .Where(chunk => Chunks.ContainsKey(chunk) == false)
            .OrderBy(chunk => Vector3.Distance(pos, chunk))
            .ToList();
    }

    //  Gets chunk data out of range of the player to a list.
    private List<Vector3Int> GetChunksOutOfRange(List<Vector3Int> chunksInRange, Vector3Int pos)
    {
        List<Vector3Int> chunksToRemove = new List<Vector3Int>();

        foreach
        (
            Vector3Int chunk in Chunks.Keys
            .Where(renderer => chunksInRange
            .Contains(renderer) == false)
        )
        {
            if (Chunks.ContainsKey(chunk))
            {
                chunksToRemove.Add(chunk);
            }
        }

        return chunksToRemove;
    }

    //  Removes all chunks and data when regen button is pressed.
    public void Regenerate()
    {
        RemoveAllChunkData();
        RemoveAllChunks();
        WorldData.ChunkDataToCreate.Clear();
        WorldData.ChunksToCreate.Clear();
        GenerateWorld();
    }

    //  Adds all chunk data not already in the remove data list, to regenerate a new world.
    public void RemoveAllChunkData()
    {
        foreach
        (
            Vector3Int chunk in ChunkDataList.Keys
            .Where(chunk => WorldData.ChunkDataToRemove
            .Contains(chunk) == false)
        )
        {
            WorldData.ChunkDataToRemove.Add(chunk);
        }
    }

    //  Adds all chunks not already in the remove chunks list, to regenerate a new world.
    public void RemoveAllChunks()
    {
        foreach
        (
            Vector3Int chunk in Chunks.Keys
            .Where(chunk => WorldData.ChunksToRemove
            .Contains(chunk) == false)
        )
        {
            WorldData.ChunksToRemove.Add(chunk);
        }
    }

    //  Sets a leaf voxel for every position in the chunk leaves list.
    private void GenerateLeaves(ChunkData chunk)
    {
        foreach (Vector3Int leaf in chunk.TreeData.Leaves)
        {
            ChunkFunctions.SetVoxelType(chunk, leaf, VoxelType.Leaves);
        }
    }

    //  Loops through the data list to set the seeds of all the data.
    public void ChangeWorldSeed()
    {
        foreach (NoiseData noiseData in AllNoiseData)
        {
            noiseData.GetSeed(Mathf.RoundToInt(SeedSlider.value));
        }
    }

    public void ChangeRenderDist()
    {
        ChunkRenderDist = Mathf.RoundToInt(RenderDistSlider.value);
    }

    //  Gets the chunk data of the voxel, and attempts to set the voxel.
    internal void SetVoxelType(Vector3Int pos, VoxelType newVoxelType)
    {
        //  Finds the corect chunk data.
        ChunkData chunkData = GetChunkData(pos);

        //  Attempts to find the voxel position in the chunk and sets the voxel.
        if (chunkData != null)
        {
            Vector3Int voxelPos = ChunkFunctions.GetVoxelChunkPos(chunkData, pos);
            ChunkFunctions.SetVoxelType(chunkData, voxelPos, newVoxelType);
        }
    }

    //  Tries to find the chunk from the position of the voxel.
    private ChunkData GetChunkData(Vector3Int pos)
    {
        Vector3Int chunkPos = ChunkFunctions.GetChunkPosFromVoxel(this, pos.x, pos.y, pos.z);

        ChunkData chunkData = null;

        ChunkDataList.TryGetValue(chunkPos, out chunkData);

        return chunkData;
    }

    //  Returns the chunk position of a voxel within that chunk.
    public Vector3Int GetChunkPosFromVoxelPos(Vector3Int pos)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(pos.x / (float)ChunkWidth)     * ChunkWidth,
            y = Mathf.FloorToInt(pos.y / (float)ChunkHeight)    * ChunkHeight,
            z = Mathf.FloorToInt(pos.z / (float)ChunkWidth)     * ChunkWidth
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