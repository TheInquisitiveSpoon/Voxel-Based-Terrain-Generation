//  BlockData.cs - Script for creating a Unity object containing individual block data.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Creates a Unity inspector menu
[CreateAssetMenu(fileName = "Block Data", menuName = "Block data")]

//  CLASSES:
//  Class to handle Texture data of each block.
public class VoxelData : ScriptableObject
{
    public float                TextureSizeX;
    public float                TextureSizeY;
    public List<TextureData>    DataList;
}

//  Class to handle individual block data for storing in the Data List.
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