using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public GameObject chunk;
    public GameObject[,,] chunks;
    public int chunkSize = 16;

    public byte[,,] data;
    public int worldX = 16;
    public int worldY = 16;
    public int worldZ = 16;

    public float noiseScale = 1;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public AnimationCurve meshHeightCurve;
    public float heightThreshold = .75f;

    // Use this for initialization
    void Start()
    {
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        ClearChunks();

        data = new byte[worldX, worldY, worldZ];

        float[,] noiseMap = Noise.GenerateNoiseMap(worldX, worldZ, seed, noiseScale, offset, octaves, persistance, lacunarity,meshHeightCurve,heightThreshold);


        // Add Tunnel
        int baseHeight = Mathf.FloorToInt(meshHeightCurve.keys[1].time * worldY) + 1;
        
        for (int x = 0; x < worldX; x++)
        {
            for (int y = 0; y < worldY; y++)
            {
                for (int z = 0; z < worldZ; z++)
                {
                    if (y <= noiseMap[x, z] * worldY)
                    {
                        data[x, y, z] = 1;
                    }
                    else
                    {
                        data[x, y, z] = 0;
                    }

                    if (x >= 30 && x <= 40 && y >= baseHeight && y <= baseHeight + 2)
                    {
                        data[x, y, z] = 0;
                    }
                    /*
                    else if (y <= dirt + stone)
                    { //Changed this line thanks to a comment
                        data[x, y, z] = 2;
                    }*/
                }
            }
        }

        chunks = new GameObject[Mathf.FloorToInt(worldX / chunkSize),
            Mathf.FloorToInt(worldY / chunkSize),
            Mathf.FloorToInt(worldZ / chunkSize)];

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    chunks[x, y, z] = Instantiate( chunk,
                     new Vector3(x * chunkSize, y * chunkSize, z * chunkSize),
                     new Quaternion(0, 0, 0, 0),
                     this.transform) as GameObject;

                    Chunk newChunkScript = chunks[x, y, z].GetComponent("Chunk") as Chunk;

                    newChunkScript.worldGO = gameObject;
                    newChunkScript.chunkSize = chunkSize;
                    newChunkScript.chunkX = x * chunkSize;
                    newChunkScript.chunkY = y * chunkSize;
                    newChunkScript.chunkZ = z * chunkSize;

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    

    public byte Block(int x, int y, int z)
    {
        if (y >= worldY)
            return (byte)0;
        else if (x >= worldX || x < 0 || y < 0 || z >= worldZ || z < 0)
            return (byte)1;

        return data[x, y, z];
    }
    
    void ClearChunks()
    {
        if (chunks != null)
        {
            foreach (GameObject chunk in chunks)
            {
                DestroyImmediate(chunk);
            }
            chunks = null;
        }

    }

    private void OnDestroy()
    {
        ClearChunks();
    }
    

}
