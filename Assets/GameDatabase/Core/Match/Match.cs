
using System.Drawing;

using UnityEngine;

public static class Match
{
    public static int[,] Create(Vector2Int size, int seed, int count)
    {
        int[,] array = new int[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Random.InitState(seed);
                int index = Random.Range(0, count);
                array[x, y] = index;
                seed += 2;
            }
        }
        return array;
    }

    public static T[,] Bundle<T>(T[] data, int[,] index)
    {
        int xCount = index.GetLength(0);
        int yCount = index.GetLength(1);

        T[,] slots = new T[xCount, yCount];

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                slots[x, y] = data[index[x, y]];
            }
        }
        return slots;
    }
}
