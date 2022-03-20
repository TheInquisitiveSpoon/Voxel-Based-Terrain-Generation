using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundHandler : LayerHandler
{
    public VoxelType UndergroundVoxel;
    public VoxelType DeepUndergroundVoxel;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y < groundLevel && pos.y > groundLevel - 5)
        {
            ChunkFunctions.SetVoxelType(data, pos, UndergroundVoxel);
            return true;
        }
        else if (pos.y < groundLevel && pos.y < groundLevel - 5)
        {
            ChunkFunctions.SetVoxelType(data, pos, DeepUndergroundVoxel);
            return true;
        }

        return false;
    }
}
