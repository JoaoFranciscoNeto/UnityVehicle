using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[ExecuteInEditMode]
public class World : MonoBehaviour
{

    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh,
        Voxel
    }

    public DrawMode drawMode;

    public GameObject chunk;
    public GameObject[,,] chunks;
    public int chunkSize = 16;

    public byte[,,] data;
    public int worldX = 16;
    public int worldY = 16;
    public int worldZ = 16;

    public float noiseScale = 1;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public AnimationCurve heightCurve;

    public TerrainType[] regions;

    public bool autoUpdate;
    

    float[,] filter = new float[5, 5]
    {
        {0,     1,      1,      1,      0},
        {1,     1,      1,      1,      1},
        {1,     1,      1,      1,      1},
        {1,     1,      1,      1,      1},
        {0,     1,      1,      1,      0},
    };
    Vector2[] plateues;


    // Use this for initialization
    void Start()
    {

        GenerateWorld();
        
    }

    public void GenerateWorld()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(worldX, worldZ, seed, noiseScale, offset, octaves, persistance, lacunarity, heightCurve);

        Color[] colorMap = new Color[worldX * worldZ];
        for (int z = 0; z < worldZ; z++)
        {
            for (int x = 0; x < worldX; x++)
            {
                float currentHeight = noiseMap[x, z];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[z * worldX + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, worldX, worldZ));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap,worldY), TextureGenerator.TextureFromColorMap(colorMap, worldX, worldZ));
        }


        /*
        ClearChunks();

        data = new byte[worldX, worldY, worldZ];

        //ProcessMap(noiseMap);
        
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= noiseMap[x, z] * worldY)
                    {
                        data[x, y, z] = 1;
                    }
                    else
                    {
                        data[x, y, z] = 0;
                    }
                    

                    texture.SetPixel(x, z, Color.Lerp(Color.white, Color.black, noiseMap[x, z]));
                }
            }
        }


        texture.Apply();

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
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 viewerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        foreach (GameObject c in chunks)
        {
            c.GetComponent<Chunk>().UpdateVisibility(viewerPosition);
        }*/
    }

    void ProcessMap(float[,] map)
    {
        plateues = MapProcessing.filterMax(map, filter);

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

    private void OnDrawGizmos()
    { /*
        foreach (Vector2 p in startPoints)
        {
            Gizmos.DrawSphere(new Vector3(p.x, 32, p.y), 1);
        }
        */
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
