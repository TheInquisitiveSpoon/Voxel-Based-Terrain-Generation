using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX, tileSizeY;
    public static Dictionary<VoxelType, TextureData> blockTextureDataDictionary = new Dictionary<VoxelType, TextureData>();
    public BlockData textureData;

    private void Awake()
    {
        foreach (var item in textureData.DataList)
        {
            if (blockTextureDataDictionary.ContainsKey(item.VoxelType) == false)
            {
                blockTextureDataDictionary.Add(item.VoxelType, item);
            };
        }
        tileSizeX = textureData.TextureSizeX;
        tileSizeY = textureData.TextureSizeY;
    }
}