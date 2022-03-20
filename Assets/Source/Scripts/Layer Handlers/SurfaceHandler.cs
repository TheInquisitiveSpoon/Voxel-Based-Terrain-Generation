using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceHandler : LayerHandler
{
    public World World;
    public VoxelType SurfaceVoxel;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (pos.y == groundLevel)
        {
            if (pos.y <= World.WaterLevel + 1)
            {
                ChunkFunctions.SetVoxelType(data, pos, VoxelType.Sand);
                return true;
            }
            else
            {
                ChunkFunctions.SetVoxelType(data, pos, SurfaceVoxel);
                return true;
            }
        }

        return false;
    }
}
