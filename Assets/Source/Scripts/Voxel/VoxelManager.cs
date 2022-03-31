//  VoxelManager.cs - Component to hold voxel data, as well as handling texturing of each voxel.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class VoxelManager : MonoBehaviour
{
    //  REFERENCES:
    public VoxelData                                    TextureData;

    //  VARIABLES:
    public static Dictionary<VoxelType, TextureData>    VoxelTextures   = new Dictionary<VoxelType, TextureData>();

    public static float                                 TextureBuffer   = 0.001f;
    public static float                                 TileWidth;
    public static float                                 TileHeight;

    //  FUNCTIONS:
    //  Function to load texture data when script is loaded.
    private void Awake()
    {
        foreach (TextureData texture in TextureData.DataList)
        {
            if (VoxelTextures.ContainsKey(texture.VoxelType) == false)
            {
                VoxelTextures.Add(texture.VoxelType, texture);
            };
        }

        TileWidth   = TextureData.TextureSizeX;
        TileHeight  = TextureData.TextureSizeY;
    }
}