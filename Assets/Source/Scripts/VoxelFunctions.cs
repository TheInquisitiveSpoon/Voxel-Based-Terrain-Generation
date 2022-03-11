using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelFunctions
{
    private static Direction[] directions =
    {
        Direction.Backward,
        Direction.Downwards,
        Direction.Forward,
        Direction.Left,
        Direction.Right,
        Direction.Upwards
    };

    public static Vector3Int GetVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Upwards => Vector3Int.up,
            Direction.Downwards => Vector3Int.down,
            Direction.Right => Vector3Int.right,
            Direction.Left => Vector3Int.left,
            Direction.Forward => Vector3Int.forward,
            Direction.Backward => Vector3Int.back,
            _ => throw new Exception("Invalid input direction")
        };
    }

    public static MeshHandler GetMeshData
        (ChunkData chunk, int x, int y, int z, MeshHandler meshData, VoxelType blockType)
    {
        if (blockType == VoxelType.Air || blockType == VoxelType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + GetVector(direction);
            var neighbourBlockType = ChunkFunctions.GetVoxelTypeFromPos(chunk, neighbourBlockCoordinates.x, neighbourBlockCoordinates.y, neighbourBlockCoordinates.z);

            if (neighbourBlockType != VoxelType.Nothing && VoxelManager.blockTextureDataDictionary[neighbourBlockType].IsSolid == false)
            {

                if (blockType == VoxelType.Water)
                {
                    if (neighbourBlockType == VoxelType.Air)
                        meshData.WaterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.WaterMesh, blockType);
                }
                else
                {
                    meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, blockType);
                }

            }
        }

        return meshData;
    }

    public static MeshHandler GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshHandler meshData, VoxelType blockType)
    {
        GetFaceVertices(direction, x, y, z, meshData, blockType);
        meshData.AddQuadTriangles(VoxelManager.blockTextureDataDictionary[blockType].GeneratesCollider);
        meshData.UVs.AddRange(FaceUVs(direction, blockType));


        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshHandler meshData, VoxelType blockType)
    {
        var generatesCollider = VoxelManager.blockTextureDataDictionary[blockType].GeneratesCollider;
        //order of vertices matters for the normals and how we render the mesh
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
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, VoxelType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, blockType);

        UVs[0] = new Vector2(VoxelManager.tileSizeX * tilePos.x + VoxelManager.tileSizeX - VoxelManager.textureOffset,
            VoxelManager.tileSizeY * tilePos.y + VoxelManager.textureOffset);

        UVs[1] = new Vector2(VoxelManager.tileSizeX * tilePos.x + VoxelManager.tileSizeX - VoxelManager.textureOffset,
            VoxelManager.tileSizeY * tilePos.y + VoxelManager.tileSizeY - VoxelManager.textureOffset);

        UVs[2] = new Vector2(VoxelManager.tileSizeX * tilePos.x + VoxelManager.textureOffset,
            VoxelManager.tileSizeY * tilePos.y + VoxelManager.tileSizeY - VoxelManager.textureOffset);

        UVs[3] = new Vector2(VoxelManager.tileSizeX * tilePos.x + VoxelManager.textureOffset,
            VoxelManager.tileSizeY * tilePos.y + VoxelManager.textureOffset);

        return UVs;
    }

    public static Vector2Int TexturePosition(Direction direction, VoxelType blockType)
    {
        return direction switch
        {
            Direction.Upwards => VoxelManager.blockTextureDataDictionary[blockType].TopTexture,
            Direction.Downwards => VoxelManager.blockTextureDataDictionary[blockType].BottomTexture,
            _ => VoxelManager.blockTextureDataDictionary[blockType].SideTexture
        };
    }
}