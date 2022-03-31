//  Biome.cs - Class to hold the generator and ground level of the biome.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class Biome
{
    //  VARIABLES:
    public BiomeGenerator   BiomeGenerator  = null;

    //  Nullable int, used when passing ground level to generation functions
    //  if passed as null, then the ground level will be calculated.
    public int?             GroundLevel     = null;

    //  CONSTRUCTOR:
    public Biome(BiomeGenerator biomeGenerator, int? groundLevel = null)
    {
        BiomeGenerator  = biomeGenerator;
        GroundLevel     = groundLevel;
    }
}
