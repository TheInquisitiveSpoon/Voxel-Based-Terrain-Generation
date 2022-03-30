using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceHandler : LayerHandler
{
    public VoxelType SurfaceVoxel;
    public VoxelType UnderwaterVoxel;
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y == groundLevel && pos.y < data.World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, UnderwaterVoxel);
            return true;
        }

        if (pos.y == groundLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, SurfaceVoxel);
            return true;
        }

        return false;
    }
}
