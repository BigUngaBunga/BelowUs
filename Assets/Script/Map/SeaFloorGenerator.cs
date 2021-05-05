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

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));
        }

        protected override void FillMapWithNoise()
        {
            int coneWidth = passagewayRadius;
            int halfOfMapWidth = noiseMap.GetLength(0) / 2;

            for (int x = 0; x < noiseMap.GetLength(0); x++)
                for (int y = 0; y < noiseMap.GetLength(1); y++)
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
            int width = noiseMap.GetLength(0) * squareSize;
            int height = noiseMap.GetLength(1) * squareSize;

            rightWall.size = leftWall.size = new Vector2(2, height);
            roof.size = new Vector2(width, 2);

            rightWall.offset = new Vector2(width / 2, height - squareSize);
            leftWall.offset = new Vector2(-width / 2, height - squareSize);
            roof.offset = new Vector2(0, (height - squareSize) * 3 / 2f);
        }
    }

}
