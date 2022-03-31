//  TreeHandler.cs - Generates noise data to place trees on grass voxels, using separation distance to limit placement.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//  CLASS:
public class TreeHandler : LayerHandler
{
    //  REFERENCES:
    public NoiseData    TreeNoiseData;
    public LeafData     LeafData;

    //  VARIABLES:
    public int          TreeHeightLimit         = 30;
    public int          MinTreeHeight           = 4;
    public int          MaxTreeHeight           = 8;
    public float        TreeSeparationDistance  = 5.0f;

    //  FUNCTIONS:
    //  Places trees using octave perlin, if the noise value is greater than 0.7.
    //  Looks through the placed trees to find neighbours, and avoids placing if the closest neighbour is too near.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Maximum y position in the chunk to search until.
        int endPosition         = data.WorldPos.y + TreeHeightLimit;

        int currentTreeHeight   = MinTreeHeight;

        //  Loops through all voxels in the y axis, determining if a tree should be placed.
        for (int i = data.WorldPos.y; i <= endPosition; i++)
        {
            //  Ensures the current voxel is above the water level, and is a grass voxel.
            if (i > data.World.WaterLevel && ChunkFunctions.GetVoxelTypeFromPos(data, pos.x, i, pos.z) == VoxelType.Grass)
            {
                //  Generates a new noise value for the current voxel.
                float noise                     = NoiseGenerator.PerlinOctave(pos.x, pos.z, TreeNoiseData);

                Vector3Int treePos              = new Vector3Int(pos.x, i, pos.z);

                //  Gets any neighbours that are within the separation distance.
                List<Vector3Int> neighbours     = data.TreeData.Trees
                    .FindAll(tree => Vector3Int.Distance(treePos, tree) < TreeSeparationDistance);

                //  Places a tree if the noise is greater than 0.7 and there aren't any neighbours too near.
                if (noise > 0.7 && neighbours.Count == 0)
                {

                    //  Adds the tree to the list of placed trees.
                    data.TreeData.Trees.Add(treePos);

                    //  Sets log voxels up to the height of the current cactus.
                    for (int j = 0; j < currentTreeHeight; j++)
                    {
                        ChunkFunctions.SetVoxelType(data, new Vector3Int(treePos.x, treePos.y + j, treePos.z), VoxelType.Log);
                    }

                    //  Adds leaf voxel positions to the leaves list based on the leaf data position, relative to the
                    //  top of the tree.
                    foreach (Vector3Int leaf in LeafData.Leaves)
                    {
                        data.TreeData.Leaves.Add(new Vector3Int
                        (
                            treePos.x + leaf.x,
                            (treePos.y + currentTreeHeight) + leaf.y,
                            treePos.z + leaf.z)
                        );
                    }

                    //  Resets the current height to the minimum if it is equal to the max, otherwise adds one.
                    currentTreeHeight = currentTreeHeight == MaxTreeHeight ? MinTreeHeight : (currentTreeHeight + 1);
                }
            }
        }

        return false;
    }
}
