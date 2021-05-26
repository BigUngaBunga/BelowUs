using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class ReefGenerator : MapGenerator
    {

        [Range(40, 60)]
        [SerializeField] protected byte minimumOpenWaterPercentage;

        [Range(40, 60)]
        [SerializeField] protected byte maximumOpenWaterPercentage;

        public byte MinimumOpenWaterPercentage => minimumOpenWaterPercentage;
        public byte MaximumOpenWaterPercentage => maximumOpenWaterPercentage;

        [SerializeField] protected uint minimumEnclaveRemovalSize;
        [SerializeField] protected uint maximumEnclaveRemovalSize;

        public uint MinimumEnclaveRemovalSize => minimumEnclaveRemovalSize;
        public uint MaximumEnclaveRemovalSize => maximumEnclaveRemovalSize;

        [Range(1, 10)]
        [SerializeField] protected byte timesToSmoothMap;

        public IEnumerator GenerateReef(MapHandler mapHandler, Vector2 mapSize, int squareSize, Random random)
        {
            this.random = random;

            yield return StartCoroutine(GenerateNoiseMap(mapSize));

            yield return StartCoroutine(GetComponent<MeshGenerator>().GenerateMesh(noiseMap, squareSize, wallTile));

            GetComponent<MapEntranceDetector>().CreateEntranceDetector(passagewayRadius, new Vector2(mapWidth, mapHeight), squareSize, mapHandler);

            if (isServer)
            {
                yield return StartCoroutine(GetComponent<ResourceGenerator>().GenerateResources(random, noiseMap, squareSize, waterTile));
                yield return StartCoroutine(GetComponent<EnemyGenerator>().GenerateEnemies(random, noiseMap, squareSize, waterTile));
            }
            
        }

        protected IEnumerator GenerateNoiseMap(Vector2 mapSize)
        {
            yield return Wait("Started counting");
            RandomizeMapVariables();
            InitiateMap(mapSize);
            AddBorderToNoiseMap(borderThickness);
            SmoothNoiseMap();
            yield return Wait("Filled noise map");

            yield return StartCoroutine(RemoveTileEnclaves());

            CreateEntranceAndExit();
            yield return StartCoroutine(ClearPathways());
        }

        protected void RandomizeMapVariables()
        {
            openWaterPercentage = random.Next(minimumOpenWaterPercentage, maximumOpenWaterPercentage);
            enclaveRemovalSize = random.Next((int)minimumEnclaveRemovalSize, (int)maximumEnclaveRemovalSize);
        }

        //Meant to consolidate the noisemap to larger chunks
        protected void SmoothNoiseMap()
        {
            for (int i = 0; i < timesToSmoothMap; i++)
                for (int x = 0; x < mapWidth; x++)
                    for (int y = 0; y < mapHeight; y++)
                    {
                        int neighbouringWallTiles = GetSurrondingwallTiles(x, y);

                        if (neighbouringWallTiles > 4)
                            noiseMap[x, y] = wallTile;
                        else if (neighbouringWallTiles < 4)
                            noiseMap[x, y] = waterTile;
                    }
        }

        protected int GetSurrondingwallTiles(int xPosition, int yPosition)
        {
            int adjacentWallCount = 0;
            for (int neighbouringX = xPosition - 1; neighbouringX <= xPosition + 1; neighbouringX++)
                for (int neighbouringY = yPosition - 1; neighbouringY <= yPosition + 1; neighbouringY++)
                    if (IsInMapRange(neighbouringX, neighbouringY) && (neighbouringX != xPosition || neighbouringY != yPosition) && noiseMap[neighbouringX, neighbouringY] == wallTile)
                        adjacentWallCount++;

            return adjacentWallCount;
        }

        protected IEnumerator RemoveTileEnclaves()
        {
            IEnumerator ReplaceSmallTileRegion(int tileTypeToRemove)
            {
                int replacingTileType = tileTypeToRemove != waterTile ? waterTile : wallTile;
                List<List<Coordinate>> tileRegions = GetRegion(tileTypeToRemove);
                foreach (List<Coordinate> tileRegion in tileRegions)
                    if (tileRegion.Count < enclaveRemovalSize)
                    {
                        foreach (Coordinate tile in tileRegion)
                            noiseMap[tile.TileX, tile.TileY] = replacingTileType;

                        yield return Wait($"Replaced tile region");
                    }
            }
            yield return StartCoroutine(ReplaceSmallTileRegion(waterTile));
            yield return StartCoroutine(ReplaceSmallTileRegion(wallTile));
        }

        protected void AddBorderToNoiseMap(int borderSize)
        {
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    if (x <= borderSize || y <= borderSize || x >= mapWidth - borderSize || y >= mapHeight - borderSize)
                        noiseMap[x, y] = wallTile;
        }
    }
}

