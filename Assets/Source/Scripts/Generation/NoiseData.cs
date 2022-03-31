//  NoiseData.cs - Scriptable object to hold noise data needed for performing octave perlin noise.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Creates a menu option in the unity editor to create a new object.
[CreateAssetMenu(fileName = "Noise Data", menuName = "Data/Noise Data")]

//  CLASS:
public class NoiseData : ScriptableObject
{
    //  VARIABLES:
    public int          Seed;
    public int          Octaves;
    public Vector2Int   Offset;
    public float        Magnitude;
    public float        Persistence;
    public float        Modifier;
    public float        PlateauExponent;

    //  FUNCTIONS:
    //  Function to set the seed of the noise.
    public void GetSeed(int seed)
    {
        Seed = seed;
    }
}