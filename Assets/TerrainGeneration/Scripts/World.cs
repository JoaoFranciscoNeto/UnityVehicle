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

    public float[,] heightMap;
    public byte[,,] data;
    public int sizeX = 16;
    public int sizeY = 16;
    public int sizeZ = 16;

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
    Queue<MapThreadInfo<ChunkMeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<ChunkMeshData>>();

    // Use this for initialization
    void Start()
    {
        /*
        GenerateMap();
        DrawMapInEditor();
        */
    }

    public void GenerateMap()
    {
        Debug.Log("Generating Map");
        GenerateMapData();
        GenerateChunkArray();
    }



    void GenerateChunkArray()
    {
        ClearChunks();

        chunks = new GameObject[Mathf.FloorToInt(sizeX / chunkSize),
            Mathf.FloorToInt(sizeY / chunkSize),
            Mathf.FloorToInt(sizeZ / chunkSize)];

        Vector3 topLeftOffset = new Vector3(-chunks.GetLength(0) * chunkSize, 0, -chunks.GetLength(2) * chunkSize );

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

    public void RequestMeshData(Vector2 chunkOffset, Action<ChunkMeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(chunkOffset, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(Vector2 chunkOffset, Action<ChunkMeshData> callback)
    {
        ChunkMeshData meshData = new VoxelGenerator(this, chunkOffset).GenerateVoxelMesh();
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<ChunkMeshData>(callback, meshData));
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
    
    // Generate heightmap from perlin noise
    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(sizeX, sizeZ, seed, noiseScale, offset, octaves, persistance, lacunarity, heightCurve);
        heightMap = noiseMap;
        return new MapData(noiseMap, Vector3.zero);
        
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
                MapThreadInfo<ChunkMeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    
    public byte Block(int x, int y, int z)
    {
        if (y >= sizeY)
            return (byte)0;
        else if (x >= sizeX || x < 0 || y < 0 || z >= sizeZ || z < 0)
            return (byte)1;

        return data[x, y, z];
    }

    public void ClearChunks()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));

        chunks = null;
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