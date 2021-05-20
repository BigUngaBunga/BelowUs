using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class MapGenerator : NetworkBehaviour
    {
        [Min(50)]
        [SerializeField] protected int mapHeight;

        [Min(50)]
        [SerializeField] protected int mapWidth;

        [SerializeField] protected string seed;
        [SerializeReference] protected bool useRandomSeed, generateExit;

        [Range(3, 10)]
        [SerializeField] protected byte borderThickness;

        [Range(0, 5)]
        [SerializeField] protected byte passagewayRadius;

        protected int openWaterPercentage;
        protected int enclaveRemovalSize;
        protected const int waterTile = 1;
        protected const int wallTile = 0;
        protected int[,] noiseMap;
        protected Random random;
        public Vector2 ExitLocation { get; protected set; }
        public Vector2 MapSize => new Vector2(mapWidth, mapHeight);

        protected struct Coordinate
        {
            public int TileX { get; }
            public int TileY { get; }

            public Coordinate(int tileX, int tileY)
            {
                TileX = tileX;
                TileY = tileY;
            }
        }

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
        protected class Room : IComparable<Room>
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
        {
            public List<Coordinate> Tiles { get; private set; }
            public List<Coordinate> EdgeTiles { get; private set; }
            public List<Room> ConnectedRooms { get; private set; }
            public int TilesInRoom { get; private set; }
            public bool IsAccesibleFromMainRoom { get; set; }
            public bool IsMainRoom { get; set; }
            public Room(List<Coordinate> tiles, int[,] map, int waterTile)
            {
                Tiles = tiles;
                TilesInRoom = tiles.Count;
                ConnectedRooms = new List<Room>();
                EdgeTiles = new List<Coordinate>();

                foreach (Coordinate tile in tiles)
                    for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
                        for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                            if (x == tile.TileX || (y == tile.TileY && IsInmapRange(x, y, map) && map[x, y] == waterTile))
                                EdgeTiles.Add(tile);
            }

            public static void ConnectRooms(Room roomA, Room roomB)
            {
                if (roomA.IsAccesibleFromMainRoom)
                    roomB.SetAccesibleFromMainRoom();
                else if (roomB.IsAccesibleFromMainRoom)
                    roomA.SetAccesibleFromMainRoom();

                roomA.ConnectedRooms.Add(roomB);
                roomB.ConnectedRooms.Add(roomA);
            }

            public bool IsConnected(Room otherRoom) => ConnectedRooms.Contains(otherRoom);

            public int CompareTo(Room other) => other.TilesInRoom.CompareTo(TilesInRoom);

            public void SetAccesibleFromMainRoom()
            {
                if (!IsAccesibleFromMainRoom)
                {
                    IsAccesibleFromMainRoom = true;
                    foreach (Room connectedRoom in ConnectedRooms)
                        connectedRoom.SetAccesibleFromMainRoom();
                }
            }

            private bool IsInmapRange(int tileX, int tileY, int[,] map) => tileX >= 0 && tileX < map.GetLength(0) && tileY >= 0 && tileY < map.GetLength(1);
        }

        public bool IsInMapRange(int tileX, int tileY) => tileX >= 0 && tileX < mapWidth && tileY >= 0 && tileY < mapHeight;

        protected WaitForSeconds Wait(string text = "") => CorutineUtilities.Wait(0.005f, text);

        protected void InitiateMap(Vector2 mapSize)
        {
            mapWidth = (int)mapSize.x;
            mapHeight = (int)mapSize.y;
            noiseMap = new int[mapWidth, mapHeight];
            FillMapWithNoise();
        }

        protected virtual void FillMapWithNoise()
        {
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    noiseMap[x, y] = (random.Next(0, 100) >= openWaterPercentage) ? wallTile : waterTile;
        }

        protected void CreateEntranceAndExit(bool randomExitPlacement = true)
        {
            int entranceSize = borderThickness - 2;
            int exitDistanceFromCorners = 2 + passagewayRadius;

            Vector2 entranceLocation = new Vector2(noiseMap.GetLength(0) / 2, noiseMap.GetLength(1) - 1);
            DrawCircle(entranceLocation, entranceSize);

            if (generateExit)
            {
                ExitLocation = randomExitPlacement
                    ? new Vector2(random.Next(exitDistanceFromCorners, noiseMap.GetLength(0) - exitDistanceFromCorners), 0)
                    : new Vector2(noiseMap.GetLength(0) / 2, 0);

                DrawCircle(ExitLocation, entranceSize);
            }

        }

        protected IEnumerator ClearPathways()
        {
            List<List<Coordinate>> waterTileRegions = GetRegion(waterTile);
            List<Room> rooms = new List<Room>();

            foreach (List<Coordinate> region in waterTileRegions)
                rooms.Add(new Room(region, noiseMap, waterTile));

            rooms.Sort();
            rooms[0].IsMainRoom = true;
            rooms[0].IsAccesibleFromMainRoom = true;

            yield return StartCoroutine(ConnectAllRooms(rooms));
        }

        protected List<List<Coordinate>> GetRegion(int tileType)
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
                            flaggedTiles[tile.TileX, tile.TileY] = 1;
                    }

            return regions;
        }

        protected List<Coordinate> GetRegionTiles(int startX, int startY)
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

                for (int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
                    for (int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
                        if (IsInMapRange(x, y) && (x == tile.TileX || y == tile.TileY) && flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                        {
                            flaggedTiles[x, y] = 1;
                            queue.Enqueue(new Coordinate(x, y));
                        }
            }

            return tiles;
        }

        protected IEnumerator ConnectAllRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false) //IEnumerator
        {
            List<Room> unConnectedRooms = new List<Room>();
            List<Room> ConnectedRooms = new List<Room>();

            if (forceAccessibilityFromMainRoom)
            {
                foreach (Room room in rooms)
                {
                    if (room.IsAccesibleFromMainRoom)
                        ConnectedRooms.Add(room);
                    else
                        unConnectedRooms.Add(room);
                }
                yield return StartCoroutine(ConnectRooms(unConnectedRooms, ConnectedRooms));
            }
            else
                yield return StartCoroutine(ConnectRooms(rooms, rooms));

            if (!forceAccessibilityFromMainRoom)
                yield return StartCoroutine(ConnectAllRooms(rooms, true));
        }

        protected IEnumerator ConnectRooms(List<Room> roomsA, List<Room> roomsB)
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
                    if (roomA.ConnectedRooms.Count > 0)
                        continue;
                }

                foreach (Room roomB in roomsB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB))
                        continue;

                    for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
                        {
                            Coordinate tileA = roomA.EdgeTiles[tileIndexA];
                            Coordinate tileB = roomB.EdgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int)(Mathf.Pow(tileA.TileX - tileB.TileX, 2) + Mathf.Pow(tileA.TileY - tileB.TileY, 2));

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

                        if (CorutineUtilities.WaitAmountOfTimes(tileIndexA, roomA.EdgeTiles.Count, 10))
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

        protected void CreatePassage(Room roomA, Room roomB, Coordinate tileA, Coordinate tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            List<Coordinate> line = GetLine(tileA, tileB);
            foreach (Coordinate point in line)
                DrawCircle(point, passagewayRadius);
        }

        protected void DrawCircle(Coordinate centre, int radius)
        {
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    if ((x * x) + (y * y) <= radius * radius)
                    {
                        int drawX = centre.TileX + x;
                        int drawY = centre.TileY + y;

                        if (IsInMapRange(drawX, drawY))
                            noiseMap[drawX, drawY] = waterTile;
                    }
        }

        protected void DrawCircle(Vector2 centre, int radius)
        {
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    if ((x * x) + (y * y) <= radius * radius)
                    {
                        int drawX = (int)centre.x + x;
                        int drawY = (int)centre.y + y;

                        if (IsInMapRange(drawX, drawY))
                            noiseMap[drawX, drawY] = waterTile;
                    }
        }

        protected List<Coordinate> GetLine(Coordinate from, Coordinate to)
        {
            List<Coordinate> line = new List<Coordinate>();
            int x = from.TileX;
            int y = from.TileY;

            int deltaX = to.TileX - from.TileX;
            int deltaY = to.TileY - from.TileY;

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
    }
}
