//  SurfaceHandler.cs - Places surface and underwater voxels at the passed voxel coordinate.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class SurfaceHandler : LayerHandler
{
    //  REFERENCES:
    public VoxelType    SurfaceVoxel;
    public VoxelType    UnderwaterVoxel;

    //  Places underwater voxels on the ground level under the water level, as well as the surface
    //  voxels on the ground level above the water level.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Places underwater voxels.
        if (pos.y == groundLevel && pos.y < data.World.WaterLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, UnderwaterVoxel);
            return true;
        }

        //  Places above water surface voxels.
        if (pos.y == groundLevel)
        {
            ChunkFunctions.SetVoxelType(data, pos, SurfaceVoxel);
            return true;
        }

        return false;
    }
}
