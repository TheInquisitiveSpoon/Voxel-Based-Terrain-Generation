//  World.Data.cs - Struct to stores lists of chunk data and renders to create or remove.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  STRUCT:
public struct WorldData
{
    public List<Vector3Int> ChunkDataToCreate;
    public List<Vector3Int> ChunksToCreate;
    public List<Vector3Int> ChunkDataToRemove;
    public List<Vector3Int> ChunksToRemove;
}