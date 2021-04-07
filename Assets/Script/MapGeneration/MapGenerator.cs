using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    public int height, width;
    public string seed;
    public bool useRandomSeed;
    private int waterTile = 0;
    private int wallTile = 1;


    [Range(0, 100)]
    public int openWaterPercentage;

    [Range (0 , 10)]
    public int timesToSmoothMap;

    [Range (0, 100)]
    public int enclaveRemovalSize;

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

    class Room
    {
        public List<Coordinate> tiles;
        public List<Coordinate> edgeTiles;
        public List<Room> adjacentRooms;
        public int tilesInRoom;

        public Room()
        {

        }

        public Room(List<Coordinate> tiles, int[,] map, int waterTile, int wallTile)
        {
            this.tiles = tiles;
            tilesInRoom = tiles.Count;
            adjacentRooms = new List<Room>();
            edgeTiles = new List<Coordinate>();

            foreach (Coordinate tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX; x++) // +1
                    for (int y = tile.tileY - 1; y <= tile.tileY; y++) // +1
                    {
                        if (x == tile.tileX || y == tile.tileY)
                            if (x >= 0 && y >= 0)
                            {
                                if (map[x, y] == waterTile)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }

                                
                    }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            roomA.adjacentRooms.Add(roomB);
            roomB.adjacentRooms.Add(roomA);
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
        FillMapWithNoise();
        SmoothNoiseMap(timesToSmoothMap);
        RemoveTileEnclaves();

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
                noiseMap[x, y] = (random.Next(0, 100) >= openWaterPercentage) ? waterTile : wallTile;
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
                    int neighbourWallTiles = GetSurrondingPixels(x, y);

                    if (neighbourWallTiles > 4)
                        noiseMap[x, y] = wallTile;
                    else if (neighbourWallTiles < 4)
                        noiseMap[x, y] = waterTile;
                }
            }
        }
    }

    int GetSurrondingPixels(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbouringX = gridX -1; neighbouringX <= gridX + 1; neighbouringX++)
            for (int neighbouringY = gridY - 1; neighbouringY <= gridY + 1; neighbouringY++)
                if (IsInMapRange(neighbouringX, neighbouringY))
                    if (neighbouringX != gridX || neighbouringY != gridY)
                        wallCount += noiseMap[neighbouringX, neighbouringY];
        return wallCount;
    }


    private void RemoveTileEnclaves()
    {
        List<Room> roomsLeft = new List<Room>();

        void ReplaceSmallTileRegion(int removeType, int replaceType)
        {
            List<List<Coordinate>> tileRegions = GetRegion(removeType);
            foreach (List<Coordinate> tileRegion in tileRegions)
            {
                if (tileRegion.Count < enclaveRemovalSize)
                    foreach (Coordinate tile in tileRegion)
                        noiseMap[tile.tileX, tile.tileY] = replaceType;
                //else
                //    roomsLeft.Add(new Room(tileRegion, noiseMap, waterTile, wallTile));
            }

            Debug.Log("Boop");
        }

        ReplaceSmallTileRegion(waterTile, wallTile);
        ReplaceSmallTileRegion(waterTile, wallTile);
        //ConnectClosestRooms(roomsLeft);
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

    private void ConnectClosestRooms(List<Room> rooms)
    {
        int closestDistance = 0;
        Coordinate bestTileA = new Coordinate();
        Coordinate bestTileB = new Coordinate();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool connectionEstablished = false;

        foreach (Room roomA in rooms)
        {
            connectionEstablished = false;
            foreach (Room roomB in rooms)
            {
                if (roomA == roomB)
                    continue;
                if (roomA.adjacentRooms.Contains(roomB))
                {
                    connectionEstablished = false;
                    break;
                }
                    

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coordinate tileA = roomA.edgeTiles[tileIndexA];
                        Coordinate tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < closestDistance || !connectionEstablished)
                        {
                            closestDistance = distanceBetweenRooms;
                            connectionEstablished = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;

                        }
                    }
            }
            Debug.Log("Another room");
        }

        if (connectionEstablished)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coordinate tileA, Coordinate tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        Debug.DrawLine(CoordinateToWorldPoint(tileA), CoordinateToWorldPoint(tileB), Color.red, 100);
    }

    Vector3 CoordinateToWorldPoint(Coordinate tile)
    {
        return new Vector3(-width / 2 + 0.5f + tile.tileX, -height / 2 + 0.5f + tile.tileY, -1);
    }
}

