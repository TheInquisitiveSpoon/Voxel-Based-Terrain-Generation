using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldData
{
    public List<Vector3Int> ChunkDataToCreate;
    public List<Vector3Int> ChunksToCreate;
    public List<Vector3Int> ChunkDataToRemove;
    public List<Vector3Int> ChunksToRemove;
}
