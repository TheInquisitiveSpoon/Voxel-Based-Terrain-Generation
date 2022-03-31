//  GameController.cs - Checks periodically for when the player has moved to a new chunk using coroutines.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  CLASS:
public class GameController : MonoBehaviour
{
    //  REFERENCES:
    public GameObject   Player;
    public World        World;

    //  VARIABLES:
    public Vector3Int   PlayerChunkCenter;
    private Vector3Int  PlayerChunkPos;
    private float       DelayTimer  = 1.0f;

    //  FUNCTIONS:
    // Update is called once per frame
    private void Awake()
    {
        CheckIfPlayerChunkChanged();
        PlayerChunkCenter = Vector3Int.zero;
    }

    //  Stops coroutines and starts another to check if the player has changed chunk after a delay.
    public void CheckIfPlayerChunkChanged()
    {
        GetPlayerChunk();
        StopAllCoroutines();
        StartCoroutine(CheckIfNewChunksNeeded());
    }

    //  Waits a specified amount of time before checking if new chunks need to be loaded.
    //  Starts the function again after the yield, and starts a new coroutine if no change has occured.
    IEnumerator CheckIfNewChunksNeeded()
    {
        //  Waits until the time has elapsed.
        yield return new WaitForSeconds(DelayTimer);

        //  Checks if the player is within the bounds of the chunk they were in previously,
        //  loading new chunks if not.
        if (Mathf.Abs(PlayerChunkCenter.x - Player.transform.position.x) > World.ChunkWidth ||
            Mathf.Abs(PlayerChunkCenter.z - Player.transform.position.z) > World.ChunkWidth)
        {
            World.LoadNewChunks();
        }
        else
        {
            //  Starts another coroutine.
            StartCoroutine(CheckIfNewChunksNeeded());
        }
    }

    //  Gets the chunk position using the player position, as well as getting the center of that chunk.
    private void GetPlayerChunk()
    {
        //  Gets the chunk position the player is in.
        PlayerChunkPos = World.GetChunkPosFromVoxelPos(Vector3Int.RoundToInt(Player.transform.position));

        //  Gets the center of the chunk the player is in.
        PlayerChunkCenter.x = PlayerChunkPos.x + World.ChunkWidth / 2;
        PlayerChunkCenter.z = PlayerChunkPos.z + World.ChunkWidth / 2;
    }
}
