using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Threading;

[ExecuteInEditMode]
public class World : MonoBehaviour
{

    public enum DrawMode
    {
        NoiseMap,
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


    public bool autoUpdate;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    // Use this for initialization
    void Start()
    {

        GenerateMapData();


        chunks = new GameObject[Mathf.FloorToInt(worldX / chunkSize),
            Mathf.FloorToInt(worldY / chunkSize),
            Mathf.FloorToInt(worldZ / chunkSize)];

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    chunks[x, y, z] = Instantiate(chunk,
                     new Vector3(x * chunkSize, y * chunkSize, z * chunkSize),
                     new Quaternion(0, 0, 0, 0),
                     transform) as GameObject;

                    VoxelChunk newChunkScript = chunks[x, y, z].GetComponent("VoxelChunk") as VoxelChunk;

                    newChunkScript.worldGO = gameObject;
                    newChunkScript.chunkSize = chunkSize;
                    newChunkScript.chunkX = x * chunkSize;
                    newChunkScript.chunkY = y * chunkSize;
                    newChunkScript.chunkZ = z * chunkSize;

                }
            }
        }
        

    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }

    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = new VoxelGenerator(this,).GenerateVoxelMesh();
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(worldX, worldZ, seed, noiseScale, offset, octaves, persistance, lacunarity, heightCurve);

        return new MapData(noiseMap, Vector3.zero);


        /*
        ClearChunks();

        data = new byte[worldX, worldY, worldZ];
        
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
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
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

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public float[,] heightMap;
    public Vector3 chunkOffset;

    public MapData(float[,] heightMap, Vector3 chunkOffset)
    {
        this.heightMap = heightMap;
        this.chunkOffset = chunkOffset;
    }
}