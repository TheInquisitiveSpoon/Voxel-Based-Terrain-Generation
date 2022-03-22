using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Player;
    public World World;

    public Vector3Int PlayerChunkPos;
    public Vector3Int PlayerChunkCenter = Vector3Int.zero;
    public float DelayTimer = 1.0f;

    // Update is called once per frame
    private void Awake()
    {
        CheckIfPlayerChunkChanged();
    }

    public void CheckIfPlayerChunkChanged()
    {
        GetPlayerChunk();
        StopAllCoroutines();
        StartCoroutine(CheckIfNewChunksNeeded());
    }

    IEnumerator CheckIfNewChunksNeeded()
    {
        yield return new WaitForSeconds(DelayTimer);

        if (Mathf.Abs(PlayerChunkCenter.x - Player.transform.position.x) > World.ChunkWidth ||
            Mathf.Abs(PlayerChunkCenter.z - Player.transform.position.z) > World.ChunkWidth)
        {
            World.LoadNewChunks(PlayerChunkPos);
        }
        else
        {
            StartCoroutine(CheckIfNewChunksNeeded());
        }
    }

    private void GetPlayerChunk()
    {
        PlayerChunkPos = World.GetChunkPosFromVoxelPos(Vector3Int.RoundToInt(Player.transform.position));
        PlayerChunkCenter.x = PlayerChunkPos.x + World.ChunkWidth / 2;
        PlayerChunkCenter.z = PlayerChunkPos.z + World.ChunkWidth / 2;
    }
}
