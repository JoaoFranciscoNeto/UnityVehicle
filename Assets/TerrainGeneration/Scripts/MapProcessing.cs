using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapProcessing {

    public static Vector2[] filterMax (float[,] input, float[,] filter)
    {
        int inputHeight = input.GetLength(0);
        int inputWidth = input.GetLength(1);

        int filterHeight = filter.GetLength(0);
        int filterWidth = filter.GetLength(1);

        List<Vector2> maxPoints = new List<Vector2>();
        float maxValue = float.MinValue;

        for (int x = Mathf.CeilToInt(filterWidth / 2f); x < inputWidth - Mathf.FloorToInt(filterWidth / 2f); x++)
        {
            for (int y = Mathf.CeilToInt(filterHeight / 2f); x < inputHeight - Mathf.FloorToInt(filterHeight / 2f); y++)
            {
                float valueAt = 0;
                for (int filterX = 0; filterX < filterWidth; filterX++)
                {
                    for (int filterY = 0; filterY < filterHeight; filterY++)
                    {
                        valueAt += input[x, y] * filter[filterX, filterY];
                    }
                }
                if (valueAt > maxValue)
                {
                    maxPoints.Clear();
                    maxPoints.Add(new Vector2(x, y));
                    maxValue = valueAt;
                } else if (valueAt == maxValue)
                {
                    maxPoints.Add(new Vector2(x, y));
                }
            }
        }

        return maxPoints.ToArray();
    }
}
