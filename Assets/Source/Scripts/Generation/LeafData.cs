using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Leaf Data", menuName = "Data/Leaf data")]

public class LeafData : ScriptableObject
{
    public List<Vector3Int> Leaves;
}