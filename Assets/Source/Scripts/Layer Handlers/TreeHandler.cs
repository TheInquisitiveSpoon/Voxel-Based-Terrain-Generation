using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeHandler : LayerHandler
{
    public NoiseData TreeNoiseData;
    public float TreeHeightLimit = 30;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (groundLevel < TreeHeightLimit && data.TreeData.Trees.Contains(new Vector2Int(data.WorldPos.x + pos.x, data.WorldPos.z + pos.z)))
        {
            Vector3Int currentPos = new Vector3Int(pos.x, groundLevel, pos.z);
            VoxelType currentVoxel = ChunkFunctions.GetVoxelTypeFromPos(data, currentPos.x, currentPos.y, currentPos.z);

            if (currentVoxel == VoxelType.Grass)
            {
                for (int i = 0; i < 5; i++)
                {
                    ChunkFunctions.SetVoxelType(data, new Vector3Int(pos.x, groundLevel + i, pos.z), VoxelType.Log);
                }
            }

            //for (int y = CurrentTreeHeight - 2; y < CurrentTreeHeight; y++)
            //{
            //    for (int x = pos.x - (CurrentTreeHeight - y); x < pos.x + (CurrentTreeHeight - y); x++)
            //    {
            //        for (int z = pos.z - (CurrentTreeHeight - y); z < pos.z + (CurrentTreeHeight - y); z++)
            //        {
            //            if (x != pos.x || z != pos.z)
            //            {
            //                data.TreeData.Leaves.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z + z));
            //            }
            //        }
            //    }
            //}
        }

        return false;
    }
}
