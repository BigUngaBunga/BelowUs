using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapHeight, mapWidth;
    [SerializeField] private int borderThickness;
    [SerializeField] private string seed;
    [SerializeReference] private bool useRandomSeed;

    [Range(40, 70)]
    [SerializeField] private int openWaterPercentage;

    [Range(1, 10)]
    [SerializeField] private int timesToSmoothMap;

    [Range(0, 100)]
    [SerializeField] private int enclaveRemovalSize;

    [Range(0, 5)]
    [SerializeField] private int passagewayRadius;

    private const int waterTile = 1;
    private const int wallTile = 0;
    private int[,] noiseMap;
    private Random random;

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

        public Room(List<Coordinate> Tiles, int[,] Map, int WaterTile)
        {
            tiles = Tiles;
            tilesInRoom = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coordinate>();

            foreach (Coordinate tile in tiles)
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                        if (x == tile.tileX || y == tile.tileY && IsInMapRange(x, y, Map) && Map[x, y] == WaterTile)
                            edgeTiles.Add(tile);
        }

        public static void ConnectRooms(Room RoomA, Room RoomB)
        {
            if (RoomA.isAccesibleFromMainRoom)
                RoomB.SetAccesibleFromMainRoom();
            else if (RoomB.isAccesibleFromMainRoom)
                RoomA.SetAccesibleFromMainRoom();

            RoomA.connectedRooms.Add(RoomB);
            RoomB.connectedRooms.Add(RoomA);
        }

        public bool IsConnected(Room OtherRoom)
        {
            return connectedRooms.Contains(OtherRoom);
        }

        public int CompareTo(Room Other)
        {
            return Other.tilesInRoom.CompareTo(tilesInRoom);
        }

        public void SetAccesibleFromMainRoom()
        {
            if (!isAccesibleFromMainRoom)
            {
                isAccesibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                    connectedRoom.SetAccesibleFromMainRoom();
            }
        }

        public bool IsInMapRange(int TileX, int TileY, int[,] Map)
        {
            return TileX >= 0 && TileX < Map.GetLength(0) && TileY >= 0 && TileY < Map.GetLength(1);
        }
    }

    public void Start()
    {
        GenerateMap();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GenerateMap();
    }

    public bool IsInMapRange(int TileX, int TileY)
    {
        return TileX >= 0 && TileX < mapWidth && TileY >= 0 && TileY < mapHeight;
    }

    private void GenerateMap()
    {
        noiseMap = new int[mapWidth, mapHeight];
        FillMapWithNoise();
        AddBorderToNoiseMap(borderThickness);
        SmoothNoiseMap(timesToSmoothMap);

        RemoveTileEnclaves();
        CreateStartAndEndRooms();
        ClearPathways();

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(noiseMap, 1, wallTile);
    }

    private void FillMapWithNoise()
    {
        if (useRandomSeed)
            seed = Environment.TickCount.ToString() + Time.deltaTime.ToString();

        random = new Random(seed.GetHashCode());

        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                noiseMap[x, y] = (random.Next(0, 100) >= openWaterPercentage) ? wallTile : waterTile;
    }

    //Meant to consolidate the noisemap to larger chunks
    private void SmoothNoiseMap(int TimesToRun)
    {
        for (int i = 0; i < TimesToRun; i++)
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

    private int GetSurrondingwallTiles(int XPosition, int YPosition)
    {
        int adjacentWallCount = 0;
        for (int neighbouringX = XPosition - 1; neighbouringX <= XPosition + 1; neighbouringX++)
            for (int neighbouringY = YPosition - 1; neighbouringY <= YPosition + 1; neighbouringY++)
                if (IsInMapRange(neighbouringX, neighbouringY) && (neighbouringX != XPosition || neighbouringY != YPosition))
                    if (noiseMap[neighbouringX, neighbouringY] == wallTile)
                        adjacentWallCount++;
        return adjacentWallCount;
    }

    private void RemoveTileEnclaves()
    {
        void ReplaceSmallTileRegion(int tileTypeToRemove)
        {
            int replacingTileType = tileTypeToRemove != waterTile ? waterTile : wallTile;
            List<List<Coordinate>> tileRegions = GetRegion(tileTypeToRemove);
            foreach (List<Coordinate> tileRegion in tileRegions)
            {
                if (tileRegion.Count < enclaveRemovalSize)
                    foreach (Coordinate tile in tileRegion)
                        noiseMap[tile.tileX, tile.tileY] = replacingTileType;
            }
        }
        ReplaceSmallTileRegion(waterTile);
        ReplaceSmallTileRegion(wallTile);
    }

    private void CreateStartAndEndRooms()
    {
        Coordinate startingRoomCoordinate, endRoomCoordinate;
        startingRoomCoordinate = new Coordinate(noiseMap.GetLength(0) / 2, noiseMap.GetLength(1) - 1);
        endRoomCoordinate = new Coordinate(random.Next(1, noiseMap.GetLength(0) - passagewayRadius - 1), 0);

        DrawCircle(startingRoomCoordinate, 1);
        DrawCircle(endRoomCoordinate, 1);
    }

    private void AddBorderToNoiseMap(int BorderSize)
    {
        for (int x = 0; x < noiseMap.GetLength(0); x++)
            for (int y = 0; y < noiseMap.GetLength(1); y++)
                if (x <= BorderSize || y <= BorderSize || x >= noiseMap.GetLength(0) - BorderSize || y >= noiseMap.GetLength(1) - BorderSize)
                    noiseMap[x, y] = wallTile;
    }

    private void ClearPathways()
    {
        List<List<Coordinate>> waterTileRegions = GetRegion(waterTile);
        List<Room> rooms = new List<Room>();

        foreach (List<Coordinate> region in waterTileRegions)
            rooms.Add(new Room(region, noiseMap, waterTile));

        rooms.Sort();
        rooms[0].isMainRoom = true;
        rooms[0].isAccesibleFromMainRoom = true;

        ConnectAllRooms(rooms);
    }

    private List<List<Coordinate>> GetRegion(int TileType)
    {
        List<List<Coordinate>> regions = new List<List<Coordinate>>();
        int[,] flaggedTiles = new int[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                if (flaggedTiles[x, y] == 0 && noiseMap[x, y] == TileType)
                {
                    List<Coordinate> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coordinate tile in newRegion)
                        flaggedTiles[tile.tileX, tile.tileY] = 1;
                }

        return regions;
    }

    private List<Coordinate> GetRegionTiles(int StartX, int StartY)
    {
        List<Coordinate> tiles = new List<Coordinate>();
        int[,] flaggedTiles = new int[mapWidth, mapHeight];
        int tileType = noiseMap[StartX, StartY];

        Queue<Coordinate> queue = new Queue<Coordinate>();
        queue.Enqueue(new Coordinate(StartX, StartY));
        flaggedTiles[StartX, StartY] = 1;

        while (queue.Count > 0)
        {
            Coordinate tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY))
                        if (flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                        {
                            flaggedTiles[x, y] = 1;
                            queue.Enqueue(new Coordinate(x, y));
                        }
        }

        return tiles;
    }

    private void ConnectAllRooms(List<Room> Rooms, bool ForceAccessibilityFromMainRoom = false)
    {
        List<Room> unconnectedRooms = new List<Room>();
        List<Room> connectedRooms = new List<Room>();

        if (ForceAccessibilityFromMainRoom)
        {
            foreach (Room room in Rooms)
            {
                if (room.isAccesibleFromMainRoom)
                    connectedRooms.Add(room);
                else
                    unconnectedRooms.Add(room);
            }
            ConnectRooms(unconnectedRooms, connectedRooms);
        }
        else
            ConnectRooms(Rooms, Rooms);

        if (!ForceAccessibilityFromMainRoom)
            ConnectAllRooms(Rooms, true);
    }

    private void ConnectRooms(List<Room> RoomsA, List<Room> RoomsB)
    {
        int closestDistance = 0;
        Coordinate bestTileA = new Coordinate();
        Coordinate bestTileB = new Coordinate();
        Room bestRoomA = null;
        Room bestRoomB = null;
        bool possibleConnectionEstablished = false;
        bool isForcingStartAccesibility = !RoomsA.Equals(RoomsB);

        foreach (Room roomA in RoomsA)
        {
            if (!isForcingStartAccesibility)
            {
                possibleConnectionEstablished = false;
                if (roomA.connectedRooms.Count > 0)
                    continue;
            }

            foreach (Room roomB in RoomsB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                    continue;

                FindClosestTile(ref bestRoomA, ref bestRoomB, ref bestTileA, ref bestTileB, ref possibleConnectionEstablished, ref closestDistance, roomA, roomB);
            }
            if (possibleConnectionEstablished && !isForcingStartAccesibility)
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }

        if (possibleConnectionEstablished && isForcingStartAccesibility)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            List<Room> rooms = RoomsA.Union(RoomsB).ToList();
            ConnectAllRooms(rooms, true);
        }
    }

    private void FindClosestTile(ref Room BestRoomA, ref Room BestRoomB, ref Coordinate BestTileA, ref Coordinate BestTileB, ref bool Connection, ref int ClosestDistance, Room RoomA, Room RoomB)
    {
        for (int tileIndexA = 0; tileIndexA < RoomA.edgeTiles.Count; tileIndexA++)
            for (int tileIndexB = 0; tileIndexB < RoomB.edgeTiles.Count; tileIndexB++)
            {
                Coordinate tileA = RoomA.edgeTiles[tileIndexA];
                Coordinate tileB = RoomB.edgeTiles[tileIndexB];
                int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                if (distanceBetweenRooms < ClosestDistance || !Connection)
                {
                    ClosestDistance = distanceBetweenRooms;
                    Connection = true;
                    BestTileA = tileA;
                    BestTileB = tileB;
                    BestRoomA = RoomA;
                    BestRoomB = RoomB;
                }
            }
    }

    private void CreatePassage(Room RoomA, Room RoomB, Coordinate TileA, Coordinate TileB)
    {
        Room.ConnectRooms(RoomA, RoomB);
        List<Coordinate> line = GetLine(TileA, TileB);
        foreach (Coordinate point in line)
            DrawCircle(point, passagewayRadius);
    }

    private void DrawCircle(Coordinate Centre, int Radius)
    {
        for (int x = -Radius; x < Radius; x++)
            for (int y = -Radius; y < Radius; y++)
                if (x * x + y * y <= Radius * Radius)
                {
                    int drawX = Centre.tileX + x;
                    int drawY = Centre.tileY + y;

                    if (IsInMapRange(drawX, drawY))
                        noiseMap[drawX, drawY] = waterTile;
                }
    }

    private List<Coordinate> GetLine(Coordinate From, Coordinate To)
    {
        List<Coordinate> line = new List<Coordinate>();
        int x = From.tileX;
        int y = From.tileY;

        int deltaX = To.tileX - From.tileX;
        int deltaY = To.tileY - From.tileY;

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
