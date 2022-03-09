//  VoxelType.cs - Enumeration of block types in the game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  ENUM:
public enum VoxelType
{
    Nothing,    //  Unpopulated block
    Air,        //  Populated but empty block
    Water,
    Dirt,
    Grass,
    Stone,
    Sand,
    Log,
    Leaves
}