using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Random = System.Random;

namespace BelowUs
{
    public class MapHandler : NetworkBehaviour
    {
        private enum MapType
        {
            Reef, SeaFloor, BossRoom
        }

        private int squareSize;
        private Random random;
        private List<MapGenerator> mapGenerators;
        [SyncVar]private string seed;
        [SerializeField] private int mapsBeforeBossMap;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 mapSize;
        [SerializeField] private bool useRandomSeed;
        [SerializeReference] private GameObject reefPrefab;
        [SerializeReference] private GameObject bossRoomPrefab;
        [SerializeReference] private GameObject seaFloorPrefab;

        public GameObject ReefPrefab => reefPrefab;

        [Server]
        private void Awake()
        {
            if (useRandomSeed)
                seed = GetSeed();
        }

        private void Start()
        {
            if (seed == null)
                seed = GetSeed();

            random = GetRandom();
            mapGenerators = new List<MapGenerator>();
            squareSize = 2;
            StartCoroutine(CreateNewMap());
        }

        private IEnumerator CreateNewMap()
        {
            yield return StartCoroutine(GenerateNextMap(true));
            StartCoroutine(GenerateNextMap());
        }

        
        public IEnumerator GenerateNextMap(bool firstMap = false)
        {
            if (firstMap)
                yield return StartCoroutine(GenerateMap(seaFloorPrefab, MapType.SeaFloor));
            else if (mapGenerators.Count >= mapsBeforeBossMap)
                yield return StartCoroutine(GenerateMap(bossRoomPrefab, MapType.BossRoom));
            else
                yield return StartCoroutine(GenerateMap(reefPrefab, MapType.Reef));
        }

        private IEnumerator GenerateMap(GameObject prefab, MapType mapType)
        {
            GameObject map = Instantiate(prefab, CalculateNextPosition(), Quaternion.identity);
            MapGenerator mapGenerator;

            if (!isServer)
                Debug.Log("Before creating");

            switch (mapType)
            {
                case MapType.SeaFloor:
                    Vector3 seaFloorSize = new Vector3(mapSize.x * 2, mapSize.y / 2);
                    SeaFloorGenerator seaFloorGenerator = map.GetComponent<SeaFloorGenerator>();
                    mapGenerator = seaFloorGenerator;
                    yield return StartCoroutine(seaFloorGenerator.GenerateSeaFloor(seaFloorSize, squareSize));
                    break;
                case MapType.BossRoom:
                    BossRoomGenerator bossRoomGenerator = map.GetComponent<BossRoomGenerator>();
                    mapGenerator = bossRoomGenerator;
                    yield return StartCoroutine(bossRoomGenerator.GenerateBossRoom(mapSize, squareSize, random));
                    break;
                default: //Reef
                    ReefGenerator reefGenerator = map.GetComponent<ReefGenerator>();
                    mapGenerator = reefGenerator;
                    yield return StartCoroutine(reefGenerator.GenerateReef(this, mapSize, squareSize, random));
                    break;
            }

            if (!isServer)
                Debug.Log("After creating");

            if (!isServer)
                Debug.Log("Nr of maps: " + mapGenerators.Count);

            mapGenerators.Add(mapGenerator);
            if (!isServer)
                Debug.Log("Added a mapGenerator, now Nr of maps: " + mapGenerators.Count);

            if (isServer)
                NetworkServer.Spawn(map);
        }

        private string GetSeed() => Environment.TickCount.ToString();

        private Random GetRandom() => new Random(seed.GetHashCode());

        private Vector3 CalculateNextPosition()
        {

            if (mapGenerators.Count > 0)
            {
                MapGenerator mapGenerator = mapGenerators[mapGenerators.Count - 1];
                Vector2 nextPosition = new Vector2(x: mapGenerator.transform.position.x + ((mapGenerator.ExitLocation.x - (mapGenerator.MapSize.x / 2)) * squareSize),
                                                    y: mapGenerator.transform.position.y - (((mapGenerator.MapSize.y / 2) + (mapSize.y / 2) - 1) * squareSize));

                return new Vector3(nextPosition.x, nextPosition.y);
            }

            return startPosition;
        }
    }

}
