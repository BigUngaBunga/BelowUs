using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    public int height, width;
    public int timesToSmoothMap;
    public string seed;
    public bool useRandomSeed;

    [Range(0f, 1f)]
    public float openWaterPercentage;

    int[,] noiseMap;

    void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        noiseMap = new int[width, height];
        FillMapWithNoise();
        SmoothNoiseMap(timesToSmoothMap);
    }

    void FillMapWithNoise()
    {
        if (useRandomSeed)
        {
            seed = Time.frameCount.ToString();
        }

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = (random.NextDouble() > openWaterPercentage) ? 1 : 0;
            }
        }
    }


    void SmoothNoiseMap(int timesToRun)
    {
        for (int i = 0; i < timesToRun; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourWallTiles = GetSurrondingPixels(x, y);

                    if (neighbourWallTiles > 4)
                        noiseMap[x, y] = 1;
                    else if (neighbourWallTiles < 4)
                        noiseMap[x, y] = 0;
                }
            }
        }
    }


    int GetSurrondingPixels(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbouringX = gridX -1; neighbouringX <= gridX + 1; neighbouringX++)
        {
            for (int neighbouringY = gridY - 1; neighbouringY <= gridY + 1; neighbouringY++)
            {
                if (neighbouringX >= 0 && neighbouringX < width && neighbouringY >= 0 && neighbouringY < height)
                    if (neighbouringX != gridX || neighbouringY != gridY)
                    {
                        wallCount += noiseMap[neighbouringX, neighbouringY];
                    }
            }
        }
        return wallCount;
    }


    private void OnDrawGizmos()
    {
        if (noiseMap != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (noiseMap[x, y] == 1) ? Color.white : Color.blue;
                    Vector3 pos = new Vector3(-width/2 + x +0.5f, -height/2 + y + 0.5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }
}

