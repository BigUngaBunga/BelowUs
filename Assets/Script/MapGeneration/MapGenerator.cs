using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public int height, width;
    public string seed;
    public bool useRandomSeed;
    private int WaterTile { get { return 1; } }
    public static int WallTile { get { return 0; }}


    [Range(40, 70)]
    public int openWaterPercentage;

    [Range(1, 10)]
    public int timesToSmoothMap;

    [Range(0, 100)]
    public int enclaveRemovalSize;

    [Range(0, 5)]
    public int passagewayRadius;

    int[,] noiseMap;

    struct Coordinate
    {
        public int tileX;
        public int tileY;

        public Coordinate(int tileX, int tileY)
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }
    }

    class Room : IComparable<Room>
    {
        public List<Coordinate> tiles;
        public List<Coordinate> edgeTiles;
        public List<Room> connectedRooms;
        public int tilesInRoom;
        public bool isMainRoom, isAccesibleFromMainRoom;

        public Room()
        {

        }

        public Room(List<Coordinate> tiles, int[,] map, int WaterTile, int WallTile)
        {
            this.tiles = tiles;
            tilesInRoom = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Coordinate>();

            foreach (Coordinate tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX; x++) // +1
                    for (int y = tile.tileY - 1; y <= tile.tileY; y++) // +1
                    {
                        if (x == tile.tileX || y == tile.tileY)
                            if (x >= 0 && y >= 0)
                            {
                                if (map[x, y] == WaterTile)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                    }
            }
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

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.tilesInRoom.CompareTo(tilesInRoom);
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
    }

    void Start()
    {
        GenerateMap();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GenerateMap();
    }

    public bool IsInMapRange(int tileX, int tileY)
    {
        return tileX >= 0 && tileX < width && tileY >= 0 && tileY < height;
    }

    private void GenerateMap()
    {
        noiseMap = new int[width, height];
        int borderThickness = 2;
        FillMapWithNoise();
        SmoothNoiseMap(timesToSmoothMap);
        //AddBorderToNoiseMap(borderThickness);
        RemoveTileEnclaves();
        CreateStartAndEndRooms();
        ClearPathways();

        noiseMap = CreateBorderedMap(borderThickness);
        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(noiseMap, 1);
    }

    void FillMapWithNoise()
    {
        if (useRandomSeed)
            seed = System.DateTime.Now.Millisecond.ToString() + Time.time.ToString();

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                noiseMap[x, y] = (random.Next(0, 100) >= openWaterPercentage) ? WallTile : WaterTile;
    }

    //Meant to consolidate the noisemap to larger chunks
    void SmoothNoiseMap(int timesToRun)
    {
        for (int i = 0; i < timesToRun; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourWallTiles = GetSurrondingWallTiles(x, y);

                    if (neighbourWallTiles > 4)
                        noiseMap[x, y] = WallTile;
                    else if (neighbourWallTiles < 4)
                        noiseMap[x, y] = WaterTile;
                }
            }
        }
    }
    
    int GetSurrondingWallTiles(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbouringX = gridX - 1; neighbouringX <= gridX + 1; neighbouringX++)
            for (int neighbouringY = gridY - 1; neighbouringY <= gridY + 1; neighbouringY++)
                if (IsInMapRange(neighbouringX, neighbouringY))
                    if (neighbouringX != gridX || neighbouringY != gridY)
                        if (noiseMap[neighbouringX, neighbouringY] == WallTile)
                            wallCount++;
        return wallCount;
    }
    
    private void RemoveTileEnclaves()
    {
        void ReplaceSmallTileRegion(int removeType)
        {
            int replaceType = removeType != WaterTile ? WaterTile : WallTile;
            List<List<Coordinate>> tileRegions = GetRegion(removeType);
            foreach (List<Coordinate> tileRegion in tileRegions)
            {
                if (tileRegion.Count < enclaveRemovalSize)
                    foreach (Coordinate tile in tileRegion)
                        noiseMap[tile.tileX, tile.tileY] = replaceType;
            }
        }
        ReplaceSmallTileRegion(WaterTile);
        ReplaceSmallTileRegion(WallTile);
    }
    
    private void CreateStartAndEndRooms()
    {
        System.Random random = new System.Random(seed.GetHashCode());
        Coordinate startingRoomCoordinate, endRoomCoordinate;

        startingRoomCoordinate = new Coordinate(noiseMap.GetLength(0) / 2, 0);
        endRoomCoordinate.tileX = random.Next(1, noiseMap.GetLength(0) - passagewayRadius - 1);
        endRoomCoordinate.tileY = noiseMap.GetLength(1) - 1;

        DrawCircle(startingRoomCoordinate, 1);
        DrawCircle(endRoomCoordinate, 1);
    }

    private void AddBorderToNoiseMap(int BorderSize)
    {
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                if (x <= BorderSize || y <= BorderSize || x >= noiseMap.GetLength(0) - BorderSize || y >= noiseMap.GetLength(1) - BorderSize)
                {
                    noiseMap[x, y] = WallTile;
                }
            }
        }
    }

    private int[,] CreateBorderedMap(int borderSize)
    {
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = noiseMap[x - borderSize, y - borderSize];
                }
                else
                    borderedMap[x, y] = WallTile;
            }
        }

        return borderedMap;
    }


    private void ClearPathways()
    {
        List<List<Coordinate>> tileRegions = GetRegion(WaterTile);
        List<Room> rooms = new List<Room>();

        foreach (List<Coordinate> region in tileRegions)
        {
            rooms.Add(new Room(region, noiseMap, WaterTile, WallTile));
        }

        rooms.Sort();
        rooms[0].isMainRoom = true;
        rooms[0].isAccesibleFromMainRoom = true;

        ConnectAllRooms(rooms);
    }

    private List<List<Coordinate>> GetRegion(int tileType)
    {
        List<List<Coordinate>> regions = new List<List<Coordinate>>();
        int[,] flaggedTiles = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                {
                    List<Coordinate> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coordinate tile in newRegion)
                    {
                        flaggedTiles[tile.tileX, tile.tileY] = 1;
                    }
                }

        return regions;
    }

    private List<Coordinate> GetRegionTiles(int startX, int startY)
    {
        List<Coordinate> tiles = new List<Coordinate>();
        int[,] flaggedTiles = new int[width, height];
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
                    if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY))
                        if (flaggedTiles[x, y] == 0 && noiseMap[x, y] == tileType)
                        {
                            flaggedTiles[x, y] = 1;
                            queue.Enqueue(new Coordinate(x, y));
                        }
        }

        return tiles;
    }

    private void ConnectAllRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false)
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
            ConnectRooms(unconnectedRooms, connectedRooms);
        }
        else
            ConnectRooms(rooms, rooms);

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectAllRooms(rooms, true);
        }
    }

    private void ConnectRooms(List<Room> roomsA, List<Room> roomsB)
    {
        int closestDistance = 0;
        Coordinate bestTileA = new Coordinate();
        Coordinate bestTileB = new Coordinate();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionEstablished = false;
        bool isForcingStartAccesibility = !roomsA.Equals(roomsB);

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

                FindClosestTile(ref bestRoomA, ref bestRoomB, ref bestTileA, ref bestTileB, ref possibleConnectionEstablished, ref closestDistance, roomA, roomB);
            }
            if (possibleConnectionEstablished && !isForcingStartAccesibility)
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }

        if (possibleConnectionEstablished && isForcingStartAccesibility)
        { 
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            List<Room> rooms = roomsA.Union(roomsB).ToList();
            ConnectAllRooms(rooms, true);
        }
    }

    private void FindClosestTile(ref Room bestRoomA, ref Room bestRoomB, ref Coordinate bestTileA, ref Coordinate bestTileB, ref bool connection, ref int closestDistance, Room roomA, Room roomB)
    {
        for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
            for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
            {
                Coordinate tileA = roomA.edgeTiles[tileIndexA];
                Coordinate tileB = roomB.edgeTiles[tileIndexB];
                int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                if (distanceBetweenRooms < closestDistance || !connection)
                {
                    closestDistance = distanceBetweenRooms;
                    connection = true;
                    bestTileA = tileA;
                    bestTileB = tileB;
                    bestRoomA = roomA;
                    bestRoomB = roomB;
                }
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
        {
            for (int y = -radius; y < radius; y++)
            {
                if (x*x + y*y <= radius * radius)
                {
                    int drawX = centre.tileX + x;
                    int drawY = centre.tileY + y;

                    if (IsInMapRange(drawX, drawY))
                        noiseMap[drawX, drawY] = WaterTile;
                }
            }
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
            {
                y += step;
            }
            else
            {
                x += step;
            }

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

