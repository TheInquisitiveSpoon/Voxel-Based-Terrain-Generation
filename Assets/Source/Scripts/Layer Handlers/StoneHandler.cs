using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneHandler : LayerHandler
{
    public float stoneLimit = 0.3f;

    public World World;
    public NoiseData stoneNoiseData;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        float stoneNoise = NoiseGenerator.PerlinOctave(data.WorldPos.x + pos.x, data.WorldPos.z + pos.z, stoneNoiseData);

        int endPosition = groundLevel;
        if (data.WorldPos.y < 0)
        {
            endPosition = data.WorldPos.y + data.Height;
        }

        if (stoneNoise > stoneLimit)
        {
            for (int i = data.WorldPos.y; i <= endPosition; i++)
            {
                Vector3Int stonePos = new Vector3Int(pos.x, i, pos.z);
                ChunkFunctions.SetVoxelType(data, stonePos, VoxelType.Stone);
            }
            return true;
        }
        return false;
    }
}
