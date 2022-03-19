using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LayerHandler : MonoBehaviour
{
    public LayerHandler Next;

    public bool HandleLayer(ChunkData data, Vector3Int pos, int GroundLevel)
    {
        if (AttemptHandle(data, pos, GroundLevel)) { return true; }
        if (Next != null) { return Next.HandleLayer(data, pos, GroundLevel); }
        return false;
    }

    protected abstract bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel);
}
