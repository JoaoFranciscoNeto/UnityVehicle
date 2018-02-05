using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(World))]
public class InfiniteTerrain : MonoBehaviour {

    public static float maxViewDistance = 200;

    

    public Transform viewer;
    public static Vector3 viewerPosition;
    int chunkSize;
    int chunksVisibleinViewDist;

    Dictionary<Vector2, VoxelChunk> terrainChunkDictionary = new Dictionary<Vector2, VoxelChunk>();

	// Use this for initialization
	void Start () {
        chunkSize = GetComponent<World>().chunkSize;
        chunksVisibleinViewDist = Mathf.RoundToInt(maxViewDistance / chunkSize);
	}

    void UpdateVisibleChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleinViewDist; yOffset <= chunksVisibleinViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleinViewDist; xOffset <= chunksVisibleinViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {

                } else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new VoxelChunk());
                }
            }
        }
    }

    public class TerrainChunk
    {

    }
}
