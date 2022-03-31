//  WaterHandler.cs - Places water voxels if the voxel is greater than the ground level, and less than the water level.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class WaterHandler : LayerHandler
{
    public VoxelType    WaterVoxel;

    //  Places water voxel if the voxel coordinate is greater than the ground level, and less than the water level.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Places water voxel.
        if (pos.y > groundLevel && pos.y <= data.World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, WaterVoxel);
            return true;
        }

        return false;
    }
}
