//  ChunkFunctions.cs - Functions for manipulating chunk data, usuable in multiple classes by being a static class.
//  Some functions used from the following source:
//  Accessible at:  https://web.archive.org/web/20150214125646/http://alexstv.com/index.php/posts/unity-voxel-block-tutorial-pt-3

using System;
using UnityEngine;

//  CLASS:
public static class ChunkFunctions
{
    //  FUNCTIONS:
    //  Gets the mesh data of all the voxels within the chunk, based on the voxel type.
    public static MeshHandler GetMeshData(ChunkData chunkData)
    {
        //  Creates new mesh handler to manage primary mesh vertex, UV, triangles and collider information for the mesh.
        MeshHandler    meshData             = new MeshHandler();

        //  Creates mesh handler for the water submesh.
        meshData.WaterMesh                  = new MeshHandler();

        //  Loops through chunk retrieving mesh data for each voxel.
        for (int i = 0; i < chunkData.Voxels.Length; i++)
        {
            //  Gets voxel vector from 1D array using chunk pos and index. 
            Vector3Int pos     = GetPosFromIndex(chunkData, i);

            meshData            = VoxelFunctions.GetMeshData(chunkData, pos.x, pos.y, pos.z, meshData, chunkData.Voxels[i]);
        }

        return  meshData;
    }

    //  Returns the position of the chunk using the voxel position.
    internal static Vector3Int GetChunkPosFromVoxel(World world, int x, int y, int z)
    {
        return  new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.ChunkWidth)   * world.ChunkWidth,
            y = Mathf.FloorToInt(y / (float)world.ChunkHeight)  * world.ChunkHeight,
            z = Mathf.FloorToInt(z / (float)world.ChunkWidth)   * world.ChunkWidth
        };
    }
    
    //  Returns if voxel is within chunk bounds.
    private static bool IsVoxelInChunk(ChunkData chunkData, int chunkX, int chunkY, int chunkZ)
    {
        if (chunkX < 0 || chunkY < 0 || chunkZ < 0 ||
            chunkX >= chunkData.Width || chunkY >= chunkData.Height || chunkZ >= chunkData.Width) { return false; }

        return  true;
    }

    //  Changes the voxel type of a specific voxel in the chunk, if voxel is not in the chunk, calls the world method
    //  which is used for tree leaves, which may cross chunk borders from their parent tree.
    public static void SetVoxelType(ChunkData chunkData, Vector3Int pos, VoxelType newVoxelType)
    {
        //  Check if voxel position is present in current chunk.
        if (IsVoxelInChunk(chunkData, pos.x, pos.y, pos.z))
        {
            int index = GetIndexFromPos(chunkData, pos.x, pos.y, pos.z);
            chunkData.Voxels[index] = newVoxelType;
        }
        else
        {
            chunkData.World.SetVoxelType(pos + chunkData.WorldPos, newVoxelType);
        }
    }

    //  Gets the 1D index in the chunk of the specified voxel using the vector coordinates.
    private static int GetIndexFromPos(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.Width * y + chunkData.Width * chunkData.Height * z;
    }

    //  Gets the voxel type of a specific voxel using vector coordinates.
    public static VoxelType GetVoxelTypeFromPos(ChunkData chunkData, int x, int y, int z)
    {
        //  Returns voxel type if inside current chunk.
        if (IsVoxelInChunk(chunkData, x, y, z))
        {
            int index = GetIndexFromPos(chunkData, x, y, z);
            return chunkData.Voxels[index];
        }

        //  Returns voxel type if outside of currrent chunk using world method.
        return chunkData.World.GetVoxelTypeFromChunkPos(chunkData, chunkData.WorldPos.x + x, chunkData.WorldPos.y + y, chunkData.WorldPos.z + z);
    }

    //  Retrieves the voxel vector position from the chunk index.
    private static Vector3Int GetPosFromIndex(ChunkData chunkData, int index)
    {
        return new Vector3Int
        {
            x = index   % chunkData.Width,
            y = (index  / chunkData.Width)  % chunkData.Height,
            z = index   / (chunkData.Width  * chunkData.Height)
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