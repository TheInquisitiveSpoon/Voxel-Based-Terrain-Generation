using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneHandler : LayerHandler
{
    public float StoneLimit = 0.6f;

    public World World;
    public DomainWarping DomainWarping;
    public Toggle DomainWarpingToggle;
    public NoiseData stoneNoiseData;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        if (data.WorldPos.y > groundLevel) { return false; }

        stoneNoiseData.GetSeed(World.WorldNoiseData.Seed);

        float stoneNoise;
        if (DomainWarpingToggle.isOn) { stoneNoise = DomainWarping.GenerateDomainWarp(data.WorldPos.x + pos.x, data.WorldPos.z + pos.z, stoneNoiseData); }
        else { stoneNoise = NoiseGenerator.PerlinOctave(data.WorldPos.x + pos.x, data.WorldPos.z + pos.z, stoneNoiseData); }

        int endPosition = groundLevel;
        if (data.WorldPos.y < 0)
        {
            endPosition = data.WorldPos.y + data.Height;
        }

        if (stoneNoise > StoneLimit)
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
