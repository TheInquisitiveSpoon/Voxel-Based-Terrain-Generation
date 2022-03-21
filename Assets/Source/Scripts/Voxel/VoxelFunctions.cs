//  VoxelFunctions - Necessary function to manipulate the voxels and their data.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public static class VoxelFunctions
{
    //  ARRAY:
    //  List of possible directions for correctly drawing meshes.
    private static Direction[] directions =
    {
        Direction.Right,
        Direction.Left,
        Direction.Upwards,
        Direction.Downwards,
        Direction.Forward,
        Direction.Backward,
    };  

    //  FUNCTIONS:
    //  Returns the direction vector depending on the necessary direction passed, used for retrieving neighbour voxel.
    public static Vector3Int GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Right     => Vector3Int.right,
            Direction.Left      => Vector3Int.left,
            Direction.Upwards   => Vector3Int.up,
            Direction.Downwards => Vector3Int.down,
            Direction.Forward   => Vector3Int.forward,
            Direction.Backward  => Vector3Int.back,
        };
    }
    
    //  
    public static MeshHandler GetMeshData (ChunkData chunk, int x, int y, int z, MeshHandler meshData, VoxelType voxelType)
    {
        //  Return nothing empty mesh for Air and Nothing voxels.
        if (voxelType == VoxelType.Air || voxelType == VoxelType.Nothing) { return meshData; }

        //  Loop through each neighbour voxel to determine it's type.
        foreach (Direction direction in directions)
        {
            Vector3Int  neighbourPos    = new Vector3Int(x, y, z) + GetDirectionVector(direction);
            VoxelType   neighbourType   = ChunkFunctions.GetVoxelTypeFromPos(chunk, neighbourPos.x, neighbourPos.y, neighbourPos.z);

            //  Draws face if neighbour is any non-solid voxel.
            if (neighbourType != VoxelType.Nothing && VoxelManager.VoxelTextures[neighbourType].IsSolid == false)
            {
                //  Draws face if current voxel is water and is next to Air, or if current voxel is not water.
                if (voxelType == VoxelType.Water)
                {
                    if (neighbourType == VoxelType.Air)
                        meshData.WaterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.WaterMesh, voxelType);
                }
                else
                {
                    meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, voxelType);
                }
            }
        }

        return meshData;
    }

    //  Correctly adds the mesh vertices to the list based on the order of vertices for rendering the mesh.
    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshHandler meshData, VoxelType voxelType)
    {
        bool    generatesCollider   = VoxelManager.VoxelTextures[voxelType].GeneratesCollider;
        
        if (voxelType == VoxelType.Water)
        {
            //  Adds vertices in different orders based on the direction.
            switch (direction)
            {
                case Direction.Backward:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    break;                                         
                                                                   
                case Direction.Forward:                            
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    break;                                         
                                                                   
                case Direction.Left:                               
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    break;                                         
                                                                   
                case Direction.Right:                              
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    break;                                         
                                                                   
                case Direction.Downwards:                          
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.4f, z + 0.5f), generatesCollider);
                    break;                                         
                                                                   
                case Direction.Upwards:                            
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.4f, z - 0.5f), generatesCollider);
                    break;
            }
        }
        else
        {
            //  Adds vertices in different orders based on the direction.
            switch (direction)
            {
                case Direction.Backward:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;

                case Direction.Forward:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;

                case Direction.Left:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    break;

                case Direction.Right:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;

                case Direction.Downwards:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                    break;

                case Direction.Upwards:
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                    break;
            }
        }
    }

    //  Returns an array of UVs for the face of the mesh, using the voxel type to determine the tile position of the face on the image.
    public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
    {
        //  Array of 4 due to having 4 UVs per square face.
        Vector2[] UVs = new Vector2[4];

        //  Gets the 2D position of the tile.
        Vector2Int tilePos = TexturePosition(direction, voxelType);

        //  Sets the UVs to the correct positions, using a buffer to eliminate teared edges.
        UVs[0] = new Vector2(VoxelManager.TileWidth * tilePos.x + VoxelManager.TileWidth - VoxelManager.TextureBuffer,
            VoxelManager.TileHeight * tilePos.y + VoxelManager.TextureBuffer);
        UVs[1] = new Vector2(VoxelManager.TileWidth * tilePos.x + VoxelManager.TileWidth - VoxelManager.TextureBuffer,
            VoxelManager.TileHeight * tilePos.y + VoxelManager.TileHeight - VoxelManager.TextureBuffer);
        UVs[2] = new Vector2(VoxelManager.TileWidth * tilePos.x + VoxelManager.TextureBuffer,
            VoxelManager.TileHeight * tilePos.y + VoxelManager.TileHeight - VoxelManager.TextureBuffer);
        UVs[3] = new Vector2(VoxelManager.TileWidth * tilePos.x + VoxelManager.TextureBuffer,
            VoxelManager.TileHeight * tilePos.y + VoxelManager.TextureBuffer);

        return UVs;
    }

    //  Gets the data of the face and adds it to the mesh.
    public static MeshHandler GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshHandler meshData, VoxelType voxelType)
    {
        //  Gets the vertices of the face.
        GetFaceVertices(direction, x, y, z, meshData, voxelType);
        
        //  Splits the face into triangles and adds them to the mesh.
        meshData.AddQuadTriangles(VoxelManager.VoxelTextures[voxelType].GeneratesCollider);

        //  Gets the UVs to determine the part of the image to use for that face.
        meshData.UVs.AddRange(FaceUVs(direction, voxelType));

        return meshData;
    }

    //  Gets the correct side of the face to use for determining the UVs for the mesh.
    public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
    {
        return direction switch
        {
            Direction.Upwards => VoxelManager.VoxelTextures[voxelType].TopTexture,
            Direction.Downwards => VoxelManager.VoxelTextures[voxelType].BottomTexture,

            //  Used to handle any other direction, in this case the side faces.
            _ => VoxelManager.VoxelTextures[voxelType].SideTexture
        };
    }
}