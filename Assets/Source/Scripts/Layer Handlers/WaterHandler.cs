using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHandler : LayerHandler
{
    public VoxelType WaterVoxel;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y > groundLevel && pos.y <= data.World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, WaterVoxel);
            return true;
        }

        return false;
    }
}
