using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    //
    public float gravity = -20.0f;
    //

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    public void Awake()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {

                ChunkData data = new ChunkData(this, new Vector3Int(x * chunkSize, 0, z * chunkSize), chunkSize, chunkHeight);
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.WorldPos, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
            MeshHandler meshData = ChunkFunctions.GetMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.WorldPos, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.WorldPos, chunkRenderer);
            chunkRenderer.SetChunkData(data);
            chunkRenderer.UpdateChunk(meshData);

        }
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.WorldPos.x + x) * noiseScale, (data.WorldPos.z + z) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight);
                for (int y = 0; y < chunkHeight; y++)
                {
                    VoxelType voxelType = VoxelType.Dirt;
                    if (y > groundPosition)
                    {
                        if (y < waterThreshold)
                        {
                            voxelType = VoxelType.Water;
                        }
                        else
                        {
                            voxelType = VoxelType.Air;
                        }

                    }
                    else if (y == groundPosition)
                    {
                        voxelType = VoxelType.Grass;
                    }

                    ChunkFunctions.SetVoxelType(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    internal VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = ChunkFunctions.GetChunkPosFromVoxel(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return VoxelType.Nothing;
        Vector3Int blockInCHunkCoordinates = ChunkFunctions.GetVoxelChunkPos(containerChunk, new Vector3Int(x, y, z));
        return ChunkFunctions.GetVoxelTypeFromPos(containerChunk, blockInCHunkCoordinates.x, blockInCHunkCoordinates.y, blockInCHunkCoordinates.z);
    }
}