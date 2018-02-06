﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    

    public static ChunkMeshData GenerateTerrainMesh (float[,] heightMap, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftx = (width - 1)/-2f;
        float topLeftz = (height - 1) / 2f;
        
        ChunkMeshData meshData = new ChunkMeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftx+x, heightMap[x, y] * heightMultiplier, topLeftz-y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
    
}

public class ChunkMeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public ChunkMeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[((meshWidth - 1) * (meshHeight - 1)) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public ChunkMeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }
    
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
