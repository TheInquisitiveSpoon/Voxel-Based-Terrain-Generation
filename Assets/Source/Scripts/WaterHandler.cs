using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHandler : LayerHandler
{
    public World World;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y > groundLevel && pos.y < groundLevel + 2 && pos.y <= World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, VoxelType.Sand);
            return true;
        }
        if (pos.y > groundLevel && pos.y <= World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, VoxelType.Water);
            return true;
        }

        return false;
    }
}
