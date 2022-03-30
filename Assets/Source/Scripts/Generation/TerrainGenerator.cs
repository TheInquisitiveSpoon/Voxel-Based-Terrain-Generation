using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    public List<Vector3Int> BiomeCenters = new List<Vector3Int>();
    public List<float> TemperatureNoise = new List<float>();
    public List<float> PrecipitationNoise = new List<float>();

    [SerializeField]
    public NoiseData TemperatureNoiseData;
    public NoiseData PrecipitationNoiseData;
    public DomainWarping DomainWarping;

    [SerializeField]
    public List<BiomeData> BiomeGenerators = new List<BiomeData>();


    public ChunkData GenerateChunk(ChunkData data)
    {
        data.TreeData = new TreeData();

        for (int x = 0; x < data.Width; x++)
        {
            for (int z = 0; z < data.Width; z++)
            {
                BiomeSelector biomeSelector = GetBiomeGenerator(new Vector3Int(data.WorldPos.x + x, 0, data.WorldPos.z + z), data);
                data = biomeSelector.BiomeGenerator.GetChunkData(data, x, z, biomeSelector.GroundLevel);
            }
        }

        return data;
    }

    private BiomeSelector GetBiomeGenerator(Vector3Int worldPos, ChunkData data)
    {
        Vector2Int domainWarp = Vector2Int.RoundToInt(DomainWarping.GetIntOffset(worldPos.x, worldPos.z));
        worldPos.x += domainWarp.x;
        worldPos.z += domainWarp.y;

        List<BiomeHelper> biomes = GetBiomes(worldPos);

        BiomeGenerator biome1 = SelectBiome(biomes[0].Index);
        BiomeGenerator biome2 = SelectBiome(biomes[1].Index);

        float distance = Vector3.Distance(BiomeCenters[biomes[0].Index],
            BiomeCenters[biomes[1].Index]);

        float weight1 = biomes[0].Distance / distance;
        float weight2 = 1 - weight1;

        int groundLevel1 = biome1.GetSurfaceHeight(worldPos.x, worldPos.z, data.Height);
        int groundLevel2 = biome2.GetSurfaceHeight(worldPos.x, worldPos.z, data.Height);

        return new BiomeSelector(biome1, Mathf.RoundToInt(groundLevel1 * weight1 + groundLevel2 * weight2));
    }

    private BiomeGenerator SelectBiome(int index)
    {
        float temperature = TemperatureNoise[index];
        float precipitation = PrecipitationNoise[index];
        
        foreach (var BiomeData in BiomeGenerators)
        {
            if (temperature >= BiomeData.BiomeGenerator.MinTemperature && temperature < BiomeData.BiomeGenerator.MaxTemperature
                && precipitation >= BiomeData.BiomeGenerator.MinPrecipitation && precipitation < BiomeData.BiomeGenerator.MaxPrecipitation)
            {
                return BiomeData.BiomeGenerator;
            }
        }

        return BiomeGenerators[0].BiomeGenerator;
    }

    private List<BiomeHelper> GetBiomes(Vector3Int worldPos)
    {
        worldPos.y = 0;
        List<BiomeHelper> biomeSelectors = GetNearestBiome(worldPos);
        return biomeSelectors;
    }

    private List<BiomeHelper> GetNearestBiome(Vector3Int worldPos)
    {
        return BiomeCenters.Select((center, index) =>
        new BiomeHelper
        { 
            Index = index, 
            Distance = Vector3.Distance(center, worldPos)
        })
            .OrderBy(helper => helper.Distance)
            .Take(4)
            .ToList();
    }

    private struct BiomeHelper
    {
        public int Index;
        public float Distance;
    }

    public void GetBiomeCenters(Vector3 playerPos, int ChunkRenderDist, int ChunkWidth)
    {
        BiomeCenters = new List<Vector3Int>();
        BiomeCenters = BiomeCentering.CalculateBiomeCenters(playerPos, ChunkRenderDist, ChunkWidth);

        for (int i = 0; i < BiomeCenters.Count; i++)
        {
            Vector2Int domainWarp = DomainWarping.GetIntOffset(BiomeCenters[i].x, BiomeCenters[i].z);
            BiomeCenters[i] = new Vector3Int(BiomeCenters[i].x + domainWarp.x, 0, BiomeCenters[i].z + domainWarp.y);
        }

        TemperatureNoise = CalculateTemperature(BiomeCenters);
        PrecipitationNoise = CalculatePrecipitation(BiomeCenters);
    }

    private List<float> CalculateTemperature(List<Vector3Int> biomeCenters)
    {
        return biomeCenters
            .Select(center => NoiseGenerator.PerlinOctave(center.x, center.z, TemperatureNoiseData))
            .ToList();
    }

    private List<float> CalculatePrecipitation(List<Vector3Int> biomeCenters)
    {
        return biomeCenters
            .Select(center => NoiseGenerator.PerlinOctave(center.x, center.z, PrecipitationNoiseData))
            .ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach(Vector3Int biomeCenter in BiomeCenters)
        {
            Gizmos.DrawLine(biomeCenter, (biomeCenter + (Vector3.up * 100)));
        }
    }
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