//  VoxelData.cs - Script for creating a Unity object containing individual voxel data.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Creates a Unity inspector menu
[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel data")]

//  CLASSES:
//  Class to handle Texture data of each voxel.
public class VoxelData : ScriptableObject
{
    public float                TextureSizeX;
    public float                TextureSizeY;
    public List<TextureData>    DataList;
}

//  Class to handle individual voxel data for storing in the Data List.
[Serializable]
public class TextureData
{
    public VoxelType    VoxelType;

    public Vector2Int   TopTexture;
    public Vector2Int   BottomTexture;
    public Vector2Int   SideTexture;

    public bool         IsSolid             = true;
    public bool         GeneratesCollider   = true;
}