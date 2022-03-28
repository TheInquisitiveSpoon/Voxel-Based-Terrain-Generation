using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeHandler : LayerHandler
{
    public NoiseData TreeNoiseData;
    public LeafData LeafData;
    public int TreeHeightLimit = 30;
    public int MinTreeHeight = 4;
    public int MaxTreeHeight = 8;
    public float TreeSeparationDistance = 5.0f;

    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        int endPosition = data.WorldPos.y + TreeHeightLimit;
        int CurrentTreeHeight = MinTreeHeight;

        for (int i = data.WorldPos.y; i <= endPosition; i++)
        {
            if (i > data.World.WaterLevel && ChunkFunctions.GetVoxelTypeFromPos(data, pos.x, i, pos.z) == VoxelType.Grass)
            {
                float noise = NoiseGenerator.PerlinOctave(pos.x, pos.z, TreeNoiseData);
                Vector3Int treePos = new Vector3Int(pos.x, i, pos.z);

                List<Vector3Int> neighbours = data.TreeData.Trees.FindAll(tree => Vector3Int.Distance(treePos, tree) < TreeSeparationDistance);

                if (noise > 0.7 && neighbours.Count == 0)
                {
                    data.TreeData.Trees.Add(treePos);

                    for (int j = 0; j < CurrentTreeHeight; j++)
                    {
                        ChunkFunctions.SetVoxelType(data, new Vector3Int(treePos.x, treePos.y + j, treePos.z), VoxelType.Log);
                    }

                    foreach (Vector3Int leaf in LeafData.Leaves)
                    {
                        data.TreeData.Leaves.Add(new Vector3Int(treePos.x + leaf.x, (treePos.y + CurrentTreeHeight) + leaf.y, treePos.z + leaf.z));
                    }

                    CurrentTreeHeight = CurrentTreeHeight == MaxTreeHeight ? MinTreeHeight : (CurrentTreeHeight + 1);
                }
            }
        }

        return false;
    }
}
