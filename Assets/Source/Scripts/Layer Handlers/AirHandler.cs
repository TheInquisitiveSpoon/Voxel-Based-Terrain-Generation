using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHandler : LayerHandler
{
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y > groundLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, VoxelType.Air);
            return true;
        }

        return false;
    }
}
