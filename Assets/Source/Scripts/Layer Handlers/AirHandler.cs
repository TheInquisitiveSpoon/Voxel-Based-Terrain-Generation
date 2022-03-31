//  AirHandler.cs - Places air voxels above the ground level noise in the chunk passed.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class AirHandler : LayerHandler
{
    //  FUNCTIONS:
    //  Places air voxels at the voxel position if the y coordinate is greater than the ground level.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
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
