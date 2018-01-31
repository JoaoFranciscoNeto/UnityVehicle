using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static float GetNoise( double x, double y, double z)
    {
        return Mathf.PerlinNoise((float)x,(float)y);
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int offsetX, int offsetY)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];



        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (float)((x + offsetX) / scale);
                float sampleY = (float)((y + offsetY) / scale);
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, Vector2 offset, int octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offX = prng.Next(-100000, 100000) + offset.x;
            float offY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offX, offY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = (float)((x - halfWidth) / scale) * frequency + octaveOffsets[i].x;
                    float sampleY = (float)((y - halfHeight) / scale) * frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, Vector2 offset, int octaves, float persistance, float lacunarity, AnimationCurve heightCurve)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng;
        if (seed != -1)
        {
            prng = new System.Random(seed);
        } else
        {
            prng = new System.Random();
        }
        
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offX = prng.Next(-100000, 100000) + offset.x;
            float offY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offX, offY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = (float)((x - halfWidth) / scale) * frequency + octaveOffsets[i].x;
                    float sampleY = (float)((y - halfHeight) / scale) * frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] =heightCurve.Evaluate( Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]));
            }
        }

        return noiseMap;
    }
}
