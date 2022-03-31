//  LayerHandler.cs - Abstract class for layers, used to link layers and attempt to place voxels at each position in the chunk.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public abstract class LayerHandler : MonoBehaviour
{
    //  REFERENCES:
    public LayerHandler     NextLayer;

    //  FUNCTIONS:
    //  Attemps to handle the current voxel, if no voxel is placed then passes the voxel to the next handler.
    public bool HandleLayer(ChunkData data, Vector3Int pos, int GroundLevel)
    {
        if (AttemptHandle(data, pos, GroundLevel))  { return true; }
        if (NextLayer != null)                      { return NextLayer.HandleLayer(data, pos, GroundLevel); }

        return false;
    }

    //  Abstract function to be defined in child classes.
    protected abstract bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel);
}
