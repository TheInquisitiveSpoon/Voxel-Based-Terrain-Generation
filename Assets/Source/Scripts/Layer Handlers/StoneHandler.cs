//  StoneHandler.cs - Places stone using perlin noise to determine positions of the stone.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class StoneHandler : LayerHandler
{
    //  REFERENCES:
    public World        World;
    public NoiseData    StoneNoiseData;

    //  VARIABLES:
    //  Limits the amount of stone placed in the world.
    public float stoneLimit = 0.3f;

    //  FUNCTIONS:
    //  Returns true if the voxel was changed, and false if it was not, passing it to the next handler.
    protected override bool AttemptHandle(ChunkData data, Vector3Int pos, int groundLevel)
    {
        //  Generates a noise value for the current position.
        float stoneNoise = NoiseGenerator.PerlinOctave(data.WorldPos.x + pos.x, data.WorldPos.z + pos.z, StoneNoiseData);

        //  Limits placement up to the ground level.
        int endPosition = groundLevel;

        //  Places stone if the noise value is greater than the limit.
        if (stoneNoise > stoneLimit)
        {
            //  Loops through each voxel up to the ground level, placing stone.
            for (int i = data.WorldPos.y; i <= endPosition; i++)
            {
                ChunkFunctions.SetVoxelType(data, new Vector3Int(pos.x, i, pos.z), VoxelType.Stone);
            }

            return true;
        }

        return false;
    }
}
