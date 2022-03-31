//  UndergroundHandler.cs - Places an underground or deep underground voxel if the passed coordinate is below the ground level.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class UndergroundHandler : LayerHandler
{
    public VoxelType    UndergroundVoxel;
    public VoxelType    DeepUndergroundVoxel;

    //  Places voxel if the coordinate is beneath the ground level, using two different types of voxel.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Places underground voxel.
        if (pos.y < groundLevel && pos.y > groundLevel - 4)
        {
            ChunkFunctions.SetVoxelType(data, pos, UndergroundVoxel);
            return true;
        }
        //  Places deep underground voxel.
        else if (pos.y < groundLevel && pos.y <= groundLevel - 4)
        {
            ChunkFunctions.SetVoxelType(data, pos, DeepUndergroundVoxel);
            return true;
        }

        return false;
    }
}
