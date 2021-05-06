using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BelowUs
{
    public class MapGenerator : MonoBehaviour
    {
        [Min(50)]
        [SerializeField] private int mapHeight;

        [Min(50)]
        [SerializeField] private int mapWidth;

        [SerializeField] private string seed;
        [SerializeReference] private bool useRandomSeed, generateExit;

        [Range(0, 99)]
        [SerializeField] private byte minimumOpenWaterPercentage;

        [Range(0, 99)]
        [SerializeField] private byte maximumOpenWaterPercentage;

        public byte MinimumOpenWaterPercentage => minimumOpenWaterPercentage;
        public byte MaximumOpenWaterPercentage => maximumOpenWaterPercentage;

        [SerializeField] private uint minimumEnclaveRemovalSize;
        [SerializeField] private uint maximumEnclaveRemovalSize;

        public uint MinimumEnclaveRemovalSize => minimumEnclaveRemovalSize;
        public uint MaximumEnclaveRemovalSize => maximumEnclaveRemovalSize;

        [Range(3, 10)]
        [SerializeField] private byte borderThickness;

        [Range(1, 10)]
        [SerializeField] private byte timesToSmoothMap;

        [Range(0, 5)]
        [SerializeField] private byte passagewayRadius;

        private int openWaterPercentage;
        private int enclaveRemovalSize;
        private const int waterTile = 1;
        private const int wallTile = 0;
        private int[,] noiseMap;
        private Random random;
        public Vector2 ExitLocation { get; private set; }
        public Vector2 MapSize => new Vector2(mapWidth, mapHeight);

        private struct Coordinate
        {
            public int tileX;
            public int tileY;

            public Coordinate(int tileX, int tileY)
            {
                this.tileX = tileX;
                this.tileY = tileY;
            }
        }

        private class Room : IComparable<Room>
        {
            public List<Coordinate> tiles;
            public List<Coordinate> edgeTiles;
            public List<Room> connectedRooms;
            public int tilesInRoom;
            public bool isMainRoom, isAccesibleFromMainRoom;

            public Room(List<Coordinate> tiles, int[,] map, int waterTile)
            {
                this.tiles = tiles;
                tilesInRoom = tiles.Count;
                connectedRooms = new List<Room>();
                edgeTiles = new List<Coordinate>();

                foreach (Coordinate tile in tiles)
                    for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                        for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                            if (x == tile.tileX || y == tile.tileY && IsInmapRange(x, y, map) && map[x, y] == waterTile)
                                edgeTiles.Add(tile);
            }

            public static void ConnectRooms(Room roomA, Room roomB)
            {
                if (roomA.isAccesibleFromMainRoom)
                    roomB.SetAccesibleFromMainRoom();
                else if (roomB.isAccesibleFromMainRoom)
                    roomA.SetAccesibleFromMainRoom();

                roomA.connectedRooms.Add(roomB);
                roomB.connectedRooms.Add(roomA);
            }

            public bool IsConnected(Room otherRoom) => connectedRooms.Contains(otherRoom);

            public int CompareTo(Room other) => other.tilesInRoom.CompareTo(tilesInRoom);

            public void SetAccesibleFromMainRoom()
            {
                if (!isAccesibleFromMainRoom)
                {
                    isAccesibleFromMainRoom = true;
                    foreach (Room connectedRoom in connectedRooms)
                        connectedRoom.SetAccesibleFromMainRoom();
                }
            }

            public bool IsInmapRange(int tileX, int tileY, int[,] map) => tileX >= 0 && tileX < map.GetLength(0) && tileY >= 0 && tileY < map.GetLength(1);
        }

        public bool IsInMapRange(int tileX, int tileY) => tileX >= 0 && tileX < mapWidth && tileY >= 0 && tileY < mapHeight;

        private WaitForSeconds Wait(string text = "") => CorutineUtilities.Wait(0.005f, text);

        public IEnumerator GenerateMap(MapHandler mapHandler, Vector2 mapSize, int squareSize)
        {
            yield return Wait("Started counting");

            mapWidth = (int)mapSize.x;
            mapHeight = (int)mapSize.y;
            noiseMap = new int[mapWidth, mapHeight];
            RandomizeMapVariables();
            FillMapWithNoise();
            AddBorderToNoiseMap(borderThickness);
            SmoothNoiseMap(timesToSmoothMap);
            yield return Wait("Filled noise map");

            yield return StartCoroutine(RemoveTileEnclaves());

            CreateEntranceAndExit();
            yield return StartCoroutine(ClearPathways());

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));

            MapEntranceDetector entranceDetector = GetComponent<MapEntranceDetector>();
            entranceDetector.CreateEntranceDetector(passagewayRadius, new Vector2(mapWidth, mapHeight), squareSize, mapHandler);

            ResourceGenerator resourceGenerator = GetComponent<ResourceGenerator>();
            yield return StartCoroutine(resourceGenerator.GenerateResources(random, noiseMap, squareSize, waterTile));

            EnemyGenerator enemyGenerator = GetComponent<EnemyGenerator>();
            yield return StartCoroutine(enemyGenerator.GenerateResources(random, noiseMap, squareSize, waterTile));
        }

        public IEnumerator GenerateSeaFloor(MapHandler mapHandler, Vector2 mapSize, int squareSize)
        {
            yield return Wait("Started counting");

            mapWidth = (int)mapSize.x;
            mapHeight = (int)mapSize.y;
            noiseMap = new int[mapWidth, mapHeight];
            FillMapWithNoise(false);
            yield return Wait("Filled noise map");

            AddBoxColliders(squareSize);
            yield return Wait("Added skybox");

            CreateEntranceAndExit(false);
            yield return StartCoroutine(ClearPathways());

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));
        }

        private void RandomizeMapVariables()
        {
            if (useRandomSeed)
                seed = Environment.TickCount.ToString();
            random = new Random(seed.GetHashCode());

            openWaterPercentage = random.Next(minimumOpenWaterPercentage, maximumOpenWaterPercentage);
            enclaveRemovalSize = random.Next((int)minimumEnclaveRemovalSize, (int)maximumEnclaveRemovalSize);
        }

        private void FillMapWithNoise(bool isReef = true)
        {
            if (isReef)
                for (int x = 0; x < mapWidth; x++)
                    for (int y = 0; y < mapHeight; y++)
                        noiseMap[x, y] = (random.Next(0, 100) >= openWaterPercentage) ? wallTile : waterTile;
            else
            {
                int coneWidth = passagewayRadius;
                int halfOfMapWidth = noiseMap.GetLength(0) / 2;

                for (int x = 0; x < noiseMap.GetLength(0); x++)
                    for (int y = 0; y < noiseMap.GetLength(1); y++)
                    {
                        if ((x <= halfOfMapWidth && x > halfOfMapWidth - coneWidth - y * 2) || (x > halfOfMapWidth && x < halfOfMapWidth + coneWidth + y * 2))
                            noiseMap[x, y] = waterTile;
                        else
                            noiseMap[x, y] = wallTile;
                    }
            }
                
        }

        //Meant to consolidate the noisemap to larger chunks
        private void SmoothNoiseMap(int timesToRun)
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

        private int GetSurrondingwallTiles(int xPosition, int yPosition)
        {
            int adjacentWallCount = 0;
            for (int neighbouringX = xPosition - 1; neighbouringX <= xPosition + 1; neighbouringX++)
                for (int neighbouringY = yPosition - 1; neighbouringY <= yPosition + 1; neighbouringY++)
                    if (IsInMapRange(neighbouringX, neighbouringY) && (neighbouringX != xPosition || neighbouringY != yPosition) && noiseMap[neighbouringX, neighbouringY] == wallTile)
                        adjacentWallCount++;

            return adjacentWallCount;
        }

        private IEnumerator RemoveTileEnclaves()
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

        private void CreateEntranceAndExit(bool randomExitPlacement = true)
        {
            int entranceSize = borderThickness - 2;
            int exitDistanceFromCorners = 2 + passagewayRadius;

            Vector2 entranceLocation = new Vector2(noiseMap.GetLength(0) / 2, noiseMap.GetLength(1) - 1);
            DrawCircle(entranceLocation, entranceSize);

            if (generateExit)
            {
                if (randomExitPlacement)
                    ExitLocation = new Vector2(random.Next(exitDistanceFromCorners, noiseMap.GetLength(0) - exitDistanceFromCorners), 0);
                else
                {
                    ExitLocation = new Vector2(noiseMap.GetLength(0) / 2, 0);
                }
                    
                DrawCircle(ExitLocation, entranceSize);
            }

        }

        private void AddBorderToNoiseMap(int borderSize)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                    if (x <= borderSize || y <= borderSize || x >= noiseMap.GetLength(0) - borderSize || y >= noiseMap.GetLength(1) - borderSize)
                        noiseMap[x, y] = wallTile;
        }

        private IEnumerator ClearPathways()
        {
            List<List<Coordinate>> waterTileRegions = GetRegion(waterTile);
            List<Room> rooms = new List<Room>();

            foreach (List<Coordinate> region in waterTileRegions)
                rooms.Add(new Room(region, noiseMap, waterTile));

            rooms.Sort();
            rooms[0].isMainRoom = true;
            rooms[0].isAccesibleFromMainRoom = true;

            yield return StartCoroutine(ConnectAllRooms(rooms));
        }

        private List<List<Coordinate>> GetRegion(int tileType)
        {
            List<List<Coordinate>> regions = new List<List<Coordinate>>();
            int[,] flaggedTiles = new int[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    if (flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                    {
                        List<Coordinate> newRegion = GetRegionTiles(x, y);
                        regions.Add(newRegion);

                        foreach (Coordinate tile in newRegion)
                            flaggedTiles[tile.tileX, tile.tileY] = 1;
                    }

            return regions;
        }

        private List<Coordinate> GetRegionTiles(int startX, int startY)
        {
            List<Coordinate> tiles = new List<Coordinate>();
            int[,] flaggedTiles = new int[mapWidth, mapHeight];
            int tileType = noiseMap[startX, startY];

            Queue<Coordinate> queue = new Queue<Coordinate>();
            queue.Enqueue(new Coordinate(startX, startY));
            flaggedTiles[startX, startY] = 1;

            while (queue.Count > 0)
            {
                Coordinate tile = queue.Dequeue();
                tiles.Add(tile);

                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                        if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY) && flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                        {
                            flaggedTiles[x, y] = 1;
                            queue.Enqueue(new Coordinate(x, y));
                        }
            }

            return tiles;
        }

        private IEnumerator ConnectAllRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false) //IEnumerator
        {
            List<Room> unconnectedRooms = new List<Room>();
            List<Room> connectedRooms = new List<Room>();

            if (forceAccessibilityFromMainRoom)
            {
                foreach (Room room in rooms)
                {
                    if (room.isAccesibleFromMainRoom)
                        connectedRooms.Add(room);
                    else
                        unconnectedRooms.Add(room);
                }
                yield return StartCoroutine(ConnectRooms(unconnectedRooms, connectedRooms));
            }
            else
                yield return StartCoroutine(ConnectRooms(rooms, rooms));

            if (!forceAccessibilityFromMainRoom)
                yield return StartCoroutine(ConnectAllRooms(rooms, true));
        }

        private IEnumerator ConnectRooms(List<Room> roomsA, List<Room> roomsB)
        {
            bool possibleConnectionEstablished = false;
            bool isForcingStartAccesibility = !roomsA.Equals(roomsB);
            Room bestRoomA = null, bestRoomB = null;
            Coordinate bestTileA = new Coordinate(), bestTileB = new Coordinate();
            int closestDistance = 0;

            foreach (Room roomA in roomsA)
            {
                if (!isForcingStartAccesibility)
                {
                    possibleConnectionEstablished = false;
                    if (roomA.connectedRooms.Count > 0)
                        continue;
                }

                foreach (Room roomB in roomsB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB))
                        continue;

                    for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                        {
                            Coordinate tileA = roomA.edgeTiles[tileIndexA];
                            Coordinate tileB = roomB.edgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                            if (distanceBetweenRooms < closestDistance || !possibleConnectionEstablished)
                            {
                                closestDistance = distanceBetweenRooms;
                                possibleConnectionEstablished = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomB = roomA;
                                bestRoomA = roomB;
                            }
                        }

                        if (CorutineUtilities.WaitAmountOfTimes(tileIndexA, roomA.edgeTiles.Count, 10))
                            yield return Wait("Finding closest tiles");
                    }
                }
                if (possibleConnectionEstablished && !isForcingStartAccesibility)
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }

            if (possibleConnectionEstablished && isForcingStartAccesibility)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                List<Room> rooms = roomsA.Union(roomsB).ToList();
                yield return StartCoroutine(ConnectAllRooms(rooms, true));
            }
        }

        private void CreatePassage(Room roomA, Room roomB, Coordinate tileA, Coordinate tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            List<Coordinate> line = GetLine(tileA, tileB);
            foreach (Coordinate point in line)
                DrawCircle(point, passagewayRadius);
        }

        private void DrawCircle(Coordinate centre, int radius)
        {
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    if (x * x + y * y <= radius * radius)
                    {
                        int drawX = centre.tileX + x;
                        int drawY = centre.tileY + y;

                        if (IsInMapRange(drawX, drawY))
                            noiseMap[drawX, drawY] = waterTile;
                    }
        }

        private void DrawCircle(Vector2 centre, int radius)
        {
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    if (x * x + y * y <= radius * radius)
                    {
                        int drawX = (int)centre.x + x;
                        int drawY = (int)centre.y + y;

                        if (IsInMapRange(drawX, drawY))
                            noiseMap[drawX, drawY] = waterTile;
                    }
        }

        private List<Coordinate> GetLine(Coordinate from, Coordinate to)
        {
            List<Coordinate> line = new List<Coordinate>();
            int x = from.tileX;
            int y = from.tileY;

            int deltaX = to.tileX - from.tileX;
            int deltaY = to.tileY - from.tileY;

            int step = Math.Sign(deltaX);
            int gradientStep = Math.Sign(deltaY);

            bool inverted = false;
            int longest = Mathf.Abs(deltaX);
            int shortest = Mathf.Abs(deltaY);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(deltaY);
                shortest = Mathf.Abs(deltaX);
                step = Math.Sign(deltaY);
                gradientStep = Math.Sign(deltaX);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new Coordinate(x, y));

                if (inverted)
                    y += step;

                else
                    x += step;

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                        x += gradientStep;
                    else
                        y += gradientStep;

                    gradientAccumulation -= longest;
                }
            }

            return line;
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
            roof.offset = new Vector2(0, (height - squareSize) * 3/2f);
        }
    }
}
