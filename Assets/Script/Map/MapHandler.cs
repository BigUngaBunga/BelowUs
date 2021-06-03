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

        private readonly int squareSize = 2;
        private List<MapGenerator> mapGenerators;
        private Transform parentMap;
        private string seed;
        [SerializeField] private int mapsBeforeBossMap;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 mapSize;
        [SerializeField] private bool useRandomSeed;
        [SerializeReference] private GameObject reefPrefab;
        [SerializeReference] private GameObject bossRoomPrefab;
        [SerializeReference] private GameObject seaFloorPrefab;

        public GameObject ReefPrefab => reefPrefab;

        [Server]
        private void Start()
        {
            if (useRandomSeed || seed == null)
                seed = GetSeed();

            parentMap = GameObject.Find("Maps").transform;
            mapGenerators = new List<MapGenerator>();
            StartCoroutine(CreateNewMap());
        }

        [Server]
        private IEnumerator CreateNewMap()
        {
            yield return new WaitForSeconds(0.2f);

            yield return StartCoroutine(GenerateNextMap(true));
            StartCoroutine(GenerateNextMap());
        }

        [Server]
        public IEnumerator GenerateNextMap(bool firstMap = false)
        {
            if (firstMap)
                yield return StartCoroutine(GenerateMap(seaFloorPrefab, MapType.SeaFloor));
            else if (mapGenerators.Count >= mapsBeforeBossMap)
                yield return StartCoroutine(GenerateMap(bossRoomPrefab, MapType.BossRoom));
            else
                yield return StartCoroutine(GenerateMap(reefPrefab, MapType.Reef));
        }

        [Server]
        private IEnumerator GenerateMap(GameObject prefab, MapType mapType)
        {
            GameObject map = Instantiate(prefab, CalculateNextPosition(), Quaternion.identity, parentMap);
            NetworkServer.Spawn(map);
            MapGenerator mapGenerator;
            int hashSeed = GetSeedHash();

            switch (mapType)
            {
                case MapType.SeaFloor:
                    Vector2 seaFloorSize = new Vector2(mapSize.x * 2f, mapSize.y / 2);
                    SeaFloorGenerator seaFloorGenerator = map.GetComponent<SeaFloorGenerator>();
                    mapGenerator = seaFloorGenerator;
                    GenerateMapOnClientSide(this, mapGenerator, seaFloorSize, hashSeed);
                    yield return StartCoroutine(seaFloorGenerator.GenerateSeaFloor(seaFloorSize, squareSize));
                    break;
                case MapType.BossRoom:
                    BossRoomGenerator bossRoomGenerator = map.GetComponent<BossRoomGenerator>();
                    mapGenerator = bossRoomGenerator;
                    GenerateMapOnClientSide(this, mapGenerator, mapSize, hashSeed);
                    yield return StartCoroutine(bossRoomGenerator.GenerateBossRoom(mapSize, squareSize, GetRandom(hashSeed)));
                    break;
                default: //Reef
                    ReefGenerator reefGenerator = map.GetComponent<ReefGenerator>();
                    mapGenerator = reefGenerator;
                    GenerateMapOnClientSide(this, mapGenerator, mapSize, hashSeed);
                    yield return StartCoroutine(reefGenerator.GenerateReef(this, mapSize, squareSize, GetRandom(hashSeed)));
                    break;
            }

            mapGenerators.Add(mapGenerator);
        }

        [ClientRpc]
        private void GenerateMapOnClientSide(MapHandler mapHandler, MapGenerator mapGenerator,  Vector2 mapSize, int seed)//int squareSize,
        {
            if (isServer)
                return;

            Debug.Log(seed);
            Random localRandom = new Random(seed);

            if (mapGenerator is SeaFloorGenerator seaFloorGenerator)
                StartCoroutine(seaFloorGenerator.GenerateSeaFloor(mapSize, squareSize));
            else if (mapGenerator is BossRoomGenerator bossRoomGenerator)
                StartCoroutine(bossRoomGenerator.GenerateBossRoom(mapSize, squareSize, localRandom));
            else if (mapGenerator is ReefGenerator reefGenerator)
                StartCoroutine(reefGenerator.GenerateReef(mapHandler, mapSize, squareSize, localRandom));
        }

        private string GetSeed() => Environment.TickCount.ToString();

        private int GetSeedHash() => seed.GetHashCode() + mapGenerators.Count;

        private Random GetRandom(int seed) => new Random(seed);

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

        public void GenerateMapOnServer()
        {
            if (isServer)
                StartCoroutine(GenerateNextMap());
        }
    }

}
