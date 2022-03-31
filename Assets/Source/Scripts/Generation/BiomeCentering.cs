//  BiomeCentering.cs - Calculates biome center positions and sizes using chunk sizes and rendering distance.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public static class BiomeCentering
{
    //  Direction vectors for neighbouring biomes.
    private static List<Vector2Int> NeighbourBiomes = new List<Vector2Int>
    {
        //  N
        new Vector2Int(1, 0),

        //  NE
        new Vector2Int(1, 1),

        //  E
        new Vector2Int(0, 1),

        //  SE
        new Vector2Int(-1, 1),

        //  S
        new Vector2Int(-1, 0),

        //  SW
        new Vector2Int(-1, -1),

        //  W
        new Vector2Int(0, -1),

        //  NW
        new Vector2Int(1, -1),
    };

    //  Determines center positions for each of the biomes within a range based on chunk render distance and width.
    //  Y position does not matter as all chunk are set to 0 on the Y axis.
    public static List<Vector3Int>  CalculateBiomeCenters(Vector3 playerPos, int ChunkRenderDist, int ChunkWidth)
    {
        //  Calculates the sizes of the biomes.
        int biomeSize = ChunkRenderDist * ChunkWidth;

        //  Calculates the center position for the middle biome using the player position.
        Vector3Int centerPos = new Vector3Int
        (
            Mathf.RoundToInt(playerPos.x / biomeSize) * biomeSize,
            0,
            Mathf.RoundToInt(playerPos.z / biomeSize) * biomeSize
        );

        //  Create a hashset for the biomes to ensure a center point is not added twice.
        HashSet<Vector3Int> centerPositions = new HashSet<Vector3Int>();

        centerPositions.Add(centerPos);

        //  Loops though the neighbour direction vectors calculating center points for each of the neighbour biomes.
        foreach (Vector2Int neighbour in NeighbourBiomes)
        {
            //  Gets the neighbour and adds further neighbours by multiplying the x, z components of the center.
            Vector3Int neighbourCenterMid   = new Vector3Int
            (
                (centerPos.x + neighbour.x * biomeSize),
                0,
                (centerPos.z + neighbour.y * biomeSize)
            );

            Vector3Int neighbourCenterX2    = new Vector3Int
            (
                (centerPos.x + neighbour.x * 2 * biomeSize),
                0,
                (centerPos.z + neighbour.y * biomeSize)
            );

            Vector3Int neighbourCenterZ2    = new Vector3Int
            (
                (centerPos.x + neighbour.x * biomeSize),
                0,
                (centerPos.z + neighbour.y * 2 * biomeSize)
            );

            Vector3Int neighbourCenter2     = new Vector3Int
            (
                (centerPos.x + neighbour.x * 2 * biomeSize),
                0,
                (centerPos.z + neighbour.y * 2 * biomeSize)
            );

            //  Adds the neighbouring positions to the hashset.
            centerPositions.Add(neighbourCenterMid);
            centerPositions.Add(neighbourCenterX2);
            centerPositions.Add(neighbourCenterZ2);
            centerPositions.Add(neighbourCenter2);
        }

        return new List<Vector3Int>(centerPositions);
    }
}