using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapHandler : MonoBehaviour
    {
        private List<GameObject> maps;
        private int squareSize;
        [SerializeField] private int mapsBeforeBossMap;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 mapSize;
        [SerializeReference] private GameObject mapPrefab;
        [SerializeReference] private GameObject bossRoomPrefab;
        [SerializeReference] private GameObject seaFloorPrefab;

        public GameObject MapPrefab => mapPrefab;

        private void Start()
        {
            maps = new List<GameObject> ();
            squareSize = 2;
            StartCoroutine(CreateNewMap());
        }

        private IEnumerator CreateNewMap()
        {
            yield return StartCoroutine(CreateSeaFloor());
            GenerateNextMap();
        }

        public void GenerateNextMap()
        {
            if (maps.Count >= mapsBeforeBossMap)
                CreateBossRoom();
            else
                CreateReef();
        }

        private void CreateReef()
        {
            GameObject map = Instantiate(mapPrefab, CalculateNextPosition(), Quaternion.identity);
            StartCoroutine(map.GetComponent<ReefGenerator>().GenerateReef(this, mapSize, squareSize));
            maps.Add(map);
        }

        private void CreateBossRoom()
        {
            GameObject map = Instantiate(bossRoomPrefab, CalculateNextPosition(), Quaternion.identity);
            StartCoroutine(map.GetComponent<BossRoomGenerator>().GenerateBossRoom(mapSize, squareSize));
            maps.Add(map);
        }

        private IEnumerator CreateSeaFloor()
        {
            Vector3 seaFloorSize = new Vector3(mapSize.x * 2, mapSize.y / 2);
            GameObject map = Instantiate(seaFloorPrefab, CalculateNextPosition(), Quaternion.identity);
            yield return StartCoroutine(map.GetComponent<SeaFloorGenerator>().GenerateSeaFloor(seaFloorSize, squareSize));
            maps.Add(map);
        }

        private Vector3 CalculateNextPosition()
        {
            if (maps.Count > 0)
            {
                MapGenerator mapGenerator = maps[maps.Count - 1].GetComponent<MapGenerator>();

                Vector2 nextPosition = new Vector2(x: mapGenerator.transform.position.x + ((mapGenerator.ExitLocation.x - (mapGenerator.MapSize.x / 2)) * squareSize),
                                                    y: mapGenerator.transform.position.y - (((mapGenerator.MapSize.y / 2) + (mapSize.y / 2) - 1) * squareSize));

                return new Vector3(nextPosition.x, nextPosition.y);
            }

            return startPosition;
        }
    }

}
