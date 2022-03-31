//  NoiseGenerator.cs - Uses perlin noise generated at different octaves to determine ground height of each voxel in the world.
//  Some functions used from the following sources:
//  Accessible at:  https://adrianb.io/2014/08/09/perlinnoise.html
//                  https://www.redblobgames.com/maps/terrain-from-noise/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public static class NoiseGenerator
{
    //  Creates a noise value for the terrain by layering perlin noise using octaves.
    public static float PerlinOctave(float x, float z, NoiseData data)
    {
        //  Multiplies x, z coordinates by the magnitude then adds magnitude to the value.
        x *= data.Magnitude;
        z *= data.Magnitude;
        x += data.Magnitude;
        z += data.Magnitude;

        //  Variables needed to perform octave perlin noise.
        float finalVal = 0;
        float frequency = 1;
        float amplitude = 1;

        // Used for normalizing result to 0.0 - 1.0 range
        float amplitudeCount = 0;

        //  Calculates perlin noise for each octave then increases the frequency as well as weighting the noise based on persistance.
        for (int i = 0; i < data.Octaves; i++)
        {
            //  Gets perlin noise value.
            finalVal += Mathf.PerlinNoise(((data.Seed / 2) + data.Offset.x + x) * frequency,
                ((data.Seed / 2) + data.Offset.y + z) * frequency) * amplitude;

            //  Maintains a count of the amplitude.
            amplitudeCount += amplitude;

            //  Weights the next octave value using persistence.
            amplitude *= data.Persistence;

            //  Increases frequency per octave.
            frequency *= 2;
        }

        return finalVal / amplitudeCount;
    }

    //  Raises the elevation by a power of the noise to create plateau and steeper inclines.
    public static float Redistribute(float noise, NoiseData data)
    {
        return Mathf.Pow(noise * data.Modifier, data.PlateauExponent);
    }

    //  Remaps the 0-1 value to a value based on the minimum and maximum values.
    public static int RemapToChunkHeight(float value, float min, float max)
    {
        return (int)((min + value - 0) * (max - min) / (1 - 0));
    }
}
