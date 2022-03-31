//  VoxelData.cs - Scriptable object for containing texture and voxel information for each voxel type.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Creates a unity editor menu option for the data.
[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel data")]

//  CLASSES:
//  Class to handle texture data of each voxel.
public class VoxelData : ScriptableObject
{
    //  VARIABLES:
    public float                TextureSizeX;
    public float                TextureSizeY;
    public List<TextureData>    DataList;
}

//  Class to handle individual voxel data for storing in the data list.
[Serializable]
public class TextureData
{
    //  REFERENCES:
    public VoxelType    VoxelType;

    //  VARIABLES:
    public Vector2Int   TopTexture;
    public Vector2Int   BottomTexture;
    public Vector2Int   SideTexture;

    public bool         IsSolid             = true;
    public bool         GeneratesCollider   = true;
}