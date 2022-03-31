//  LeafData.cs - Creates a scriptable object to store the leaf vectors from the top of the tree.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Creates menu option in the unity editor to create a new data object.
[CreateAssetMenu(fileName = "Leaf Data", menuName = "Data/Leaf data")]

//  CLASS:
public class LeafData : ScriptableObject
{
    //  List of vectors for each leaf position.
    public List<Vector3Int> Leaves;
}