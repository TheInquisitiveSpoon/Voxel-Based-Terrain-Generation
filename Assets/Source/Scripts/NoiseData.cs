using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise Data", menuName = "Data/Noise Data")]
public class NoiseData : ScriptableObject
{
    public float Magnitude;
    public int Seed;
    public float Persistence;
    public float Modifier;
    public float PlateauExponent;
}