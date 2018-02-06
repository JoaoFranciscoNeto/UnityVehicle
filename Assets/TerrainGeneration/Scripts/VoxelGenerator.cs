using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerator
{
    World world;
    Vector2 chunkOffset;

    List<Vector3> newVertices = new List<Vector3>();
    List<int> newTriangles = new List<int>();
    List<Vector2> newUV = new List<Vector2>();

    // Texture
    float tUnit = .25f;
    Vector2 tBase = new Vector2(0, 0);

    int faceCount;
    int chunkSize;

    byte[,,] chunkBlocks;

    public VoxelGenerator(World world, Vector2 chunkOffset)
    {
        this.world = world;
        this.chunkOffset = chunkOffset;
        chunkSize = world.chunkSize;

    }

    public ChunkMeshData GenerateVoxelMesh()
    {
        GenerateChunkBlocks();
        GenerateMesh();
        return new ChunkMeshData(newVertices.ToArray(), newTriangles.ToArray(), newUV.ToArray());
    }

    void GenerateChunkBlocks()
    {
        chunkBlocks = new byte[chunkSize, world.sizeY, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float height = Height(x, z);

                for (int y = 0; y < world.sizeY; y++)
                {
                    if (y <= height * world.sizeY)
                    {
                        chunkBlocks[x, y, z] = 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    void GenerateMesh()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    //This code will run for every block in the chunk

                    if (Block(x, y, z) != 0)
                    {
                        //If the block is solid

                        if (Block(x, y + 1, z) == 0)
                        {
                            //Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }

                        if (Block(x, y - 1, z) == 0)
                        {
                            //Block below is air
                            CubeBot(x, y, z, Block(x, y, z));

                        }

                        if (Block(x + 1, y, z) == 0)
                        {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == 0)
                        {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == 0)
                        {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == 0)
                        {
                            //Block south is air
                            CubeSouth(x, y, z, Block(x, y, z));

                        }

                    }

                }
            }
        }

    }

    void Cube(Vector2 texturePos)
    {

        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++; // Add this line
    }

    void CubeTop(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Vector2 texturePos = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            texturePos = tBase;
        }


        Cube(texturePos);
    }
    void CubeNorth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tBase;

        Cube(texturePos);
    }
    void CubeEast(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tBase;

        Cube(texturePos);
    }
    void CubeSouth(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Vector2 texturePos;

        texturePos = tBase;

        Cube(texturePos);
    }
    void CubeWest(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        Vector2 texturePos;

        texturePos = tBase;

        Cube(texturePos);
    }
    void CubeBot(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Vector2 texturePos;

        texturePos = tBase;

        Cube(texturePos);
    }

    byte Block(int x, int y, int z)
    {
        if (y >= chunkSize)
            return (byte)0;
        else if (x >= chunkSize || x < 0 || y < 0 || z >= chunkSize || z < 0)
            return (byte)0;

        return chunkBlocks[x, y, z];
    }
    
    float Height(int x, int z)
    {
        return world.heightMap[x + (int)chunkOffset.x, z + (int)chunkOffset.y];
    }
}
