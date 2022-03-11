﻿//  ChunkFunctions.cs - Functions for manipulating chunk data.

using System;
using UnityEngine;

//  CLASS:
public static class ChunkFunctions
{
    //  FUNCTIONS:
    //  Gets the mesh data of all the voxels within the chunk, based on the voxel type.
    public static MeshHandler GetMeshData(ChunkData chunkData)
    {
        MeshHandler    meshData    = new MeshHandler(true);

        //  Loops through chunk retrieving data for each voxel.
        for (int i = 0; i < chunkData.Voxels.Length; i++)
        {
            Vector3Int  pos     = GetPosFromIndex(chunkData, i);
            meshData            = VoxelFunctions.GetMeshData(chunkData, pos.x, pos.y, pos.z, meshData, chunkData.Voxels[i]);
        }

        return  meshData;
    }

    //  Returns the position of the chunk using the voxel position.
    internal static Vector3Int GetChunkPosFromVoxel(World world, int x, int y, int z)
    {
        return  new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize)    * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight)  * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize)    * world.chunkSize
        };
    }
    
    //  Returns if voxel is within chunk bounds.
    private static bool IsVoxelInChunk(ChunkData chunkData, int chunkX, int chunkY, int chunkZ)
    {
        if (chunkX < 0 || chunkY < 0 || chunkZ < 0 ||
            chunkX >= chunkData.Width || chunkY >= chunkData.Height || chunkZ >= chunkData.Width) { return false; }

        return  true;
    }

    //  Changes the voxel type of a specific block in the chunk.
    public static void SetVoxelType(ChunkData chunkData, Vector3Int pos, VoxelType newVoxelType)
    {
        if (IsVoxelInChunk(chunkData, pos.x, pos.y, pos.z))
        {
            int index = GetIndexFromPos(chunkData, pos.x, pos.y, pos.z);
            chunkData.Voxels[index] = newVoxelType;
        }
    }

    //  Gets the index in the chunk of the specified voxel coordinates.
    private static int GetIndexFromPos(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.Width * y + chunkData.Width * chunkData.Height * z;
    }

    //  Gets the voxel type of a specific voxel using it's coordinates.
    public static VoxelType GetVoxelTypeFromPos(ChunkData chunkData, int x, int y, int z)
    {
        //  Returns voxel type if inside current chunk.
        if (IsVoxelInChunk(chunkData, x, y, z))
        {
            int index = GetIndexFromPos(chunkData, x, y, z);
            return chunkData.Voxels[index];
        }

        //  Returns voxel type if outside of currrent chunk.
        return chunkData.World.GetBlockFromChunkCoordinates(chunkData, chunkData.WorldPos.x + x, chunkData.WorldPos.y + y, chunkData.WorldPos.z + z);
    }

    //  Retrieves the voxel position from the chunk index.
    private static Vector3Int GetPosFromIndex(ChunkData chunkData, int index)
    {
        return new Vector3Int
        {
            x = index % chunkData.Width,
            y = (index / chunkData.Width) % chunkData.Height,
            z = index / (chunkData.Width * chunkData.Height)
        };
    }

    //  Gets the chunk position of the voxel.
    public static Vector3Int GetVoxelChunkPos(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.WorldPos.x,
            y = pos.y - chunkData.WorldPos.y,
            z = pos.z - chunkData.WorldPos.z
        };
    }
}