using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise Data", menuName = "Data/Noise Data")]
public class NoiseData : ScriptableObject
{
    public int Octaves;
    public float Magnitude;
    public int Seed;
    public Vector2Int Offset;
    public float Persistence;
    public float Modifier;
    public float PlateauExponent;

    public void GetSeed(int seed)
    {
        Seed = seed;
    }
}