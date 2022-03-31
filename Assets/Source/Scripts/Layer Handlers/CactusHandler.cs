//  CactusHandler.cs - Generates noise data to place cactus plants on sand voxels, using separation distance to limit placement.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//  CLASS:
public class CactusHandler : LayerHandler
{
    //  REFERENCES:
    public NoiseData    CactusNoiseData;

    //  VARIABLES:
    public int          CactusHeightLimit           = 30;
    public int          MinCactusHeight             = 4;
    public int          MaxCactusHeight             = 8;
    public float        CactusSeparationDistance    = 2.0f;

    //  FUNCTIONS:
    //  Places cacti using octave perlin, if the noise value is greater than 0.7.
    //  Looks through the placed cacti to find neighbours, and avoids placing if the closest neighbour is too near.
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Maximum y position in the chunk to search until.
        int endPosition             = data.WorldPos.y + CactusHeightLimit;

        int currentCactusHeight     = MinCactusHeight;

        //  Loops through all voxels in the y axis, determining if a cactus should be placed.
        for (int i = data.WorldPos.y; i <= endPosition; i++)
        {
            //  Ensures the current voxel is above the water level, and is a sand voxel.
            if (i >= data.World.WaterLevel && ChunkFunctions.GetVoxelTypeFromPos(data, pos.x, i, pos.z) == VoxelType.Sand)
            {
                //  Generates a new noise value for the current voxel.
                float noise                     = NoiseGenerator.PerlinOctave(pos.x, pos.z, CactusNoiseData);

                Vector3Int cactusPos            = new Vector3Int(pos.x, i, pos.z);

                //  Gets any neighbours that are within the separation distance.
                List<Vector3Int> neighbours     = data.TreeData.Trees
                    .FindAll(cactus => Vector3Int.Distance(cactusPos, cactus) < CactusSeparationDistance);

                //  Places a cactus if the noise is greater than 0.7 and there aren't any neighbours too near.
                if (noise > 0.7 && neighbours.Count == 0)
                {
                    //  Adds the cactus to the list of placed cacti.
                    data.TreeData.Trees.Add(cactusPos);

                    //  Sets cacti voxels up to the height of the current cactus.
                    for (int j = 0; j < currentCactusHeight; j++)
                    {
                        ChunkFunctions.SetVoxelType(data, new Vector3Int(cactusPos.x, cactusPos.y + j, cactusPos.z), VoxelType.Cactus);
                    }

                    //  Resets the current height to the minimum if it is equal to the max, otherwise adds one.
                    currentCactusHeight = currentCactusHeight == MaxCactusHeight ? MinCactusHeight : (currentCactusHeight + 1);
                }
            }
        }

        return false;
    }
}
