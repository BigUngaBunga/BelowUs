using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class SeaFloorGenerator : MapGenerator
    {
        [Range (0, 3)]
        [SerializeField] private float coneDesscentSharpness;

        public IEnumerator GenerateSeaFloor(Vector2 mapSize, int squareSize)
        {
            yield return Wait("Started counting");
            InitiateMap(mapSize);
            yield return Wait("Filled noise map");

            AddBoxColliders(squareSize);
            yield return Wait("Added skybox");

            CreateEntranceAndExit(false);
            yield return StartCoroutine(ClearPathways());

            yield return StartCoroutine(GetComponent<MeshGenerator>().GenerateMesh(noiseMap, squareSize, wallTile));
        }

        protected override void FillMapWithNoise()
        {
            int coneWidth = passagewayRadius;
            int halfOfMapWidth = mapWidth / 2;

            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                {
                    if ((x <= halfOfMapWidth && x > halfOfMapWidth - coneWidth - (y * coneDesscentSharpness)) || (x > halfOfMapWidth && x < halfOfMapWidth + coneWidth + (y * coneDesscentSharpness)))
                        noiseMap[x, y] = waterTile;
                    else
                        noiseMap[x, y] = wallTile;
                }
        }


        private void AddBoxColliders(int squareSize)
        {
            BoxCollider2D rightWall = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D leftWall = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D roof = gameObject.AddComponent<BoxCollider2D>();
            int width = mapWidth * squareSize;
            int height = mapHeight * squareSize;

            rightWall.size = leftWall.size = new Vector2(2, height);
            roof.size = new Vector2(width, 2);

            rightWall.offset = new Vector2(width / 2, height - squareSize);
            leftWall.offset = new Vector2(-width / 2, height - squareSize);
            roof.offset = new Vector2(0, (height - squareSize) * 3 / 2f);
        }
    }

}
