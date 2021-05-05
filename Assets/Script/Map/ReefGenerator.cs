using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class ReefGenerator : MapGenerator
    {

        [Range(0, 99)]
        [SerializeField] protected byte minimumOpenWaterPercentage;

        [Range(0, 99)]
        [SerializeField] protected byte maximumOpenWaterPercentage;

        public byte MinimumOpenWaterPercentage => minimumOpenWaterPercentage;
        public byte MaximumOpenWaterPercentage => maximumOpenWaterPercentage;

        [SerializeField] protected uint minimumEnclaveRemovalSize;
        [SerializeField] protected uint maximumEnclaveRemovalSize;

        public uint MinimumEnclaveRemovalSize => minimumEnclaveRemovalSize;
        public uint MaximumEnclaveRemovalSize => maximumEnclaveRemovalSize;

        [Range(1, 10)]
        [SerializeField] protected byte timesToSmoothMap;

        public IEnumerator GenerateReef(MapHandler mapHandler, Vector2 mapSize, int squareSize)
        {
            yield return StartCoroutine(GenerateNoiseMap(mapSize));

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));

            MapEntranceDetector entranceDetector = GetComponent<MapEntranceDetector>();
            entranceDetector.CreateEntranceDetector(passagewayRadius, new Vector2(mapWidth, mapHeight), squareSize, mapHandler);

            ResourceGenerator resourceGenerator = GetComponent<ResourceGenerator>();
            yield return StartCoroutine(resourceGenerator.GenerateResources(random, noiseMap, squareSize, waterTile));
        }

        protected IEnumerator GenerateNoiseMap(Vector2 mapSize)
        {
            yield return Wait("Started counting");
            InitiateMap(mapSize);
            AddBorderToNoiseMap(borderThickness);
            SmoothNoiseMap(timesToSmoothMap);
            yield return Wait("Filled noise map");

            yield return StartCoroutine(RemoveTileEnclaves());

            CreateEntranceAndExit();
            yield return StartCoroutine(ClearPathways());
        }

        protected override void RandomizeMapVariables()
        {
            base.RandomizeMapVariables();
            openWaterPercentage = random.Next(minimumOpenWaterPercentage, maximumOpenWaterPercentage);
            enclaveRemovalSize = random.Next((int)minimumEnclaveRemovalSize, (int)maximumEnclaveRemovalSize);
        }

        //Meant to consolidate the noisemap to larger chunks
        protected void SmoothNoiseMap(int timesToRun)
        {
            for (int i = 0; i < timesToRun; i++)
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
                        foreach (Coordinate tile in tileRegion)
                        {
                            noiseMap[tile.tileX, tile.tileY] = replacingTileType;
                            yield return Wait($"Replaced tiles {tile.tileX} {tile.tileY}");
                        }
            }
            yield return StartCoroutine(ReplaceSmallTileRegion(waterTile));
            yield return StartCoroutine(ReplaceSmallTileRegion(wallTile));
        }

        protected void AddBorderToNoiseMap(int borderSize)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                    if (x <= borderSize || y <= borderSize || x >= noiseMap.GetLength(0) - borderSize || y >= noiseMap.GetLength(1) - borderSize)
                        noiseMap[x, y] = wallTile;
        }
    }
}

