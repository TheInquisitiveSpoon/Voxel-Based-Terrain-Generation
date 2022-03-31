//  TerrainGenerator.cs - Creates biomes, center positions for those biomes, temperature and precipitation noise
//  and ground levels for each voxel position within the biomes.

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class TerrainGenerator : MonoBehaviour
{
    //  REFERENCES:
    public DomainWarping        DomainWarping;
    public NoiseData            TemperatureNoiseData;
    public NoiseData            PrecipitationNoiseData;
    public List<BiomeData>      BiomeGenerators     = new List<BiomeData>();

    //  VARIABLES:
    private List<Vector3Int>    BiomeCenters        = new List<Vector3Int>();
    private List<float>         TemperatureNoise    = new List<float>();
    private List<float>         PrecipitationNoise  = new List<float>();


    //  Generates chunk data by selecting the chosen biome for the chunk, and calculating noise values.
    public ChunkData GenerateChunk(ChunkData data)
    {
        //  Reset tree data.
        data.TreeData = new TreeData();

        //  Loop through each x, z coordinate in the chunk and get the ground height value based on
        //  the biome and neighbouring biomes.
        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                //  Calculate which biome generator to use to determine this positions noise value.
                BiomeSelector biomeSelector = GetBiomeGenerator(new Vector3Int(data.WorldPos.x + x, 0, data.WorldPos.z + z), data);
                
                //  Uses the selected biome to set each voxel based on the ground level, which is either passed
                //  or calculated within the function.
                data = biomeSelector.BiomeGenerator.GetChunkData(data, x, z, biomeSelector.GroundLevel);
            }
        }

        return data;
    }

    //  Gets the biomes of the chunk and the neighbouring biomes, using domain warping for interesting biome borders.
    //  Then generates a ground value for the chunk and nearest biome, interpolating between them to make smoother
    //  biomes borders.
    private BiomeSelector GetBiomeGenerator(Vector3Int worldPos, ChunkData data)
    {
        //  Warps the world position of the biome.
        Vector2Int domainWarp = Vector2Int.RoundToInt(DomainWarping.GetIntOffset(worldPos.x, worldPos.z));
        worldPos.x += domainWarp.x;
        worldPos.z += domainWarp.y;

        //  Gets biomes of neighbouring biomes.
        List<BiomeListData> biomes = GetBiomes(worldPos);

        //  Gets the current and nearest neighbour biome generators.
        BiomeGenerator biome1 = SelectBiome(biomes[0].Index);
        BiomeGenerator biome2 = SelectBiome(biomes[1].Index);

        //  Calculates the distance between the two biomes.
        float distance = Vector3.Distance(BiomeCenters[biomes[0].Index],
            BiomeCenters[biomes[1].Index]);

        //  Calculates interpolation weights based on the biome distance.
        float weight1 = biomes[0].Distance / distance;
        float weight2 = 1 - weight1;

        //  Generates ground levels for each biome.
        int groundLevel1 = biome1.GetSurfaceHeight(worldPos.x, worldPos.z, data.Height);
        int groundLevel2 = biome2.GetSurfaceHeight(worldPos.x, worldPos.z, data.Height);

        return new BiomeSelector(biome1, Mathf.RoundToInt(groundLevel1 * weight1 + groundLevel2 * weight2));
    }

    //  Calculates temperature and precipitation values for the current voxel by taking them from
    //  the List, then determines which biome the voxel is in, by using the biome specific values.
    private BiomeGenerator SelectBiome(int index)
    {
        //  Gets the temperature and precipitation values for the voxel.
        float temperature = TemperatureNoise[index];
        float precipitation = PrecipitationNoise[index];
        
        //  Loops through all of the generators, to determine which one to use for the current voxel.
        foreach (var BiomeData in BiomeGenerators)
        {
            if
            (
                temperature >= BiomeData.BiomeGenerator.MinTemperature &&
                temperature < BiomeData.BiomeGenerator.MaxTemperature &&
                precipitation >= BiomeData.BiomeGenerator.MinPrecipitation &&
                precipitation < BiomeData.BiomeGenerator.MaxPrecipitation
            )
            {
                return BiomeData.BiomeGenerator;
            }
        }

        return BiomeGenerators[0].BiomeGenerator;
    }

    //  Gets a list of the nearest biomes.
    private List<BiomeListData> GetBiomes(Vector3Int worldPos)
    {
        worldPos.y                          = 0;
        List<BiomeListData> biomeSelectors    = GetNearestBiome(worldPos);

        return biomeSelectors;
    }

    //  Orders the list of nearby biomes by distance from the point, then returns the
    //  nearest 4 biomes to the position.
    private List<BiomeListData> GetNearestBiome(Vector3Int worldPos)
    {
        return BiomeCenters.Select((center, index) => new BiomeListData
        { 
            Index       = index, 
            Distance    = Vector3.Distance(center, worldPos)
        })
            .OrderBy(helper => helper.Distance)
            .Take(4)
            .ToList();
    }

    //  Extra data for neighouring biomes to help select nearest biomes within the list
    //  of neighbours.
    private struct BiomeListData
    {
        public int Index;
        public float Distance;
    }

    //  Calculates the center point for each of the neighbouring biomes, using domain warping to
    //  offset the biomes centers similar to voronoi.
    public void GetBiomeCenters(Vector3 playerPos, int ChunkRenderDist, int ChunkWidth)
    {
        //  Calculates biomes center points.
        BiomeCenters = new List<Vector3Int>();
        BiomeCenters = BiomeCentering.CalculateBiomeCenters(playerPos, ChunkRenderDist, ChunkWidth);

        //  Offsets each biome center using domain warping.
        for (int i = 0; i < BiomeCenters.Count; i++)
        {
            Vector2Int domainWarp = DomainWarping.GetIntOffset(BiomeCenters[i].x, BiomeCenters[i].z);
            BiomeCenters[i] = new Vector3Int(BiomeCenters[i].x + domainWarp.x, 0, BiomeCenters[i].z + domainWarp.y);
        }

        //  Calculates the temperature and precipitation for the new biome points.
        TemperatureNoise = CalculateTemperature(BiomeCenters);
        PrecipitationNoise = CalculatePrecipitation(BiomeCenters);
    }

    //  Uses octave perlin noise to create a list of temperature values for the biomes.
    private List<float> CalculateTemperature(List<Vector3Int> biomeCenters)
    {
        return biomeCenters
            .Select(center => NoiseGenerator.PerlinOctave(center.x, center.z, TemperatureNoiseData))
            .ToList();
    }

    //  Uses octave perlin noise to create a list of precipitation values for the biomes.
    private List<float> CalculatePrecipitation(List<Vector3Int> biomeCenters)
    {
        return biomeCenters
            .Select(center => NoiseGenerator.PerlinOctave(center.x, center.z, PrecipitationNoiseData))
            .ToList();
    }

//  Only enabled if using unity editor.
#if UNITY_EDITOR
    //  Draws vertical blue lines to represent biomes centers.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //  Loops through the biome centers and draws a line using the position of the center.
        foreach(Vector3Int biomeCenter in BiomeCenters)
        {
            Gizmos.DrawLine(biomeCenter, (biomeCenter + (Vector3.up * 100)));
        }
    }
#endif
}

[Serializable]
public struct BiomeData
{
    public BiomeGenerator BiomeGenerator;
}

public class BiomeSelector
{
    public BiomeGenerator BiomeGenerator = null;
    public int? GroundLevel = null;

    public BiomeSelector(BiomeGenerator biomeGenerator, int? groundLevel = null)
    {
        BiomeGenerator = biomeGenerator;
        GroundLevel = groundLevel;
    }
}