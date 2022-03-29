using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeCentering
{
    public static List<Vector2Int> NeighbourBiomes = new List<Vector2Int>
    {
        new Vector2Int(1, 0),   //  N
        new Vector2Int(1, 1),   //  NE
        new Vector2Int(0, 1),   //  E
        new Vector2Int(-1, 1),  //  SE
        new Vector2Int(-1, 0),  //  S
        new Vector2Int(-1, -1), //  SW
        new Vector2Int(0, -1),  //  W
        new Vector2Int(1, -1),  //  NW
    };

    public static List<Vector3Int>  CalculateBiomeCenters(Vector3 playerPos, int ChunkRenderDist, int ChunkWidth)
    {
        int biomeSize = ChunkRenderDist * ChunkWidth;

        Vector3Int centerPos = new Vector3Int(Mathf.RoundToInt(playerPos.x / biomeSize) * biomeSize, 0, Mathf.RoundToInt(playerPos.z / biomeSize) * biomeSize);

        HashSet<Vector3Int> centerPositions = new HashSet<Vector3Int>();

        centerPositions.Add(centerPos);

        foreach (Vector2Int neighbour in NeighbourBiomes)
        {
            Vector3Int neighbourCenterMid = new Vector3Int((centerPos.x + neighbour.x * biomeSize), 0, (centerPos.z + neighbour.y * biomeSize));
            Vector3Int neighbourCenterX2 = new Vector3Int((centerPos.x + neighbour.x * 2 * biomeSize), 0, (centerPos.z + neighbour.y * biomeSize));
            Vector3Int neighbourCenterZ2 = new Vector3Int((centerPos.x + neighbour.x * biomeSize), 0, (centerPos.z + neighbour.y * 2 * biomeSize));
            Vector3Int neighbourCenter2 = new Vector3Int((centerPos.x + neighbour.x * 2 * biomeSize), 0, (centerPos.z + neighbour.y * 2 * biomeSize));

            centerPositions.Add(neighbourCenterMid);
            centerPositions.Add(neighbourCenterX2);
            centerPositions.Add(neighbourCenterZ2);
            centerPositions.Add(neighbourCenter2);
        }

        return new List<Vector3Int>(centerPositions);
    }
}