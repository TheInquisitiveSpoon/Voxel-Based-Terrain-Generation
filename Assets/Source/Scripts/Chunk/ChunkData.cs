//  ChunkData.cs - Data and constructor for chunks.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class ChunkData
{
    //  REFERENCES:
    public World        World;

    //  VARIABLES:
    public Vector3Int   WorldPos;
    public VoxelType[]  Voxels;
    public TreeData     TreeData;

    public int          Width       = 16;
    public int          Height      = 100;

    //  CONSTRUCTOR:
    public ChunkData(World world, Vector3Int worldPos, int width, int height)
    {
        World       = world;
        WorldPos    = worldPos;
        Width       = width;
        Height      = height;

        //  Initialises new Voxel array using the set chunk width and size.
        Voxels = new VoxelType[width * height * width];
    }
}