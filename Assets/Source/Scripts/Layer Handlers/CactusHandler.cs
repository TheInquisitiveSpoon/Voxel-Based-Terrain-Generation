using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CactusHandler : LayerHandler
{
    public NoiseData CactusNoiseData;
    public int CactusHeightLimit = 30;
    public int MinCactusHeight = 4;
    public int MaxCactusHeight = 8;
    public float CactusSeparationDistance = 2.0f;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        int endPosition = data.WorldPos.y + CactusHeightLimit;
        int CurrentCactusHeight = MinCactusHeight;

        for (int i = data.WorldPos.y; i <= endPosition; i++)
        {
            if (i >= data.World.WaterLevel && ChunkFunctions.GetVoxelTypeFromPos(data, pos.x, i, pos.z) == VoxelType.Sand)
            {
                float noise = NoiseGenerator.PerlinOctave(pos.x, pos.z, CactusNoiseData);
                Vector3Int cactusPos = new Vector3Int(pos.x, i, pos.z);

                List<Vector3Int> neighbours = data.TreeData.Trees.FindAll(cactus => Vector3Int.Distance(cactusPos, cactus) < CactusSeparationDistance);

                if (noise > 0.7 && neighbours.Count == 0)
                {
                    data.TreeData.Trees.Add(cactusPos);

                    for (int j = 0; j < CurrentCactusHeight; j++)
                    {
                        ChunkFunctions.SetVoxelType(data, new Vector3Int(cactusPos.x, cactusPos.y + j, cactusPos.z), VoxelType.Cactus);
                    }

                    CurrentCactusHeight = CurrentCactusHeight == MaxCactusHeight ? MinCactusHeight : (CurrentCactusHeight + 1);
                }
            }
        }

        return false;
    }
}
