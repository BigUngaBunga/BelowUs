using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapHandler : MonoBehaviour
    {
        private List<GameObject> maps;
        private int squareSize;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 mapSize;
        [SerializeReference] private GameObject mapPrefab;
        [SerializeReference] private GameObject seaFloorPrefab;

        private void Start()
        {
            maps = new List<GameObject> ();
            squareSize = 2;
            StartCoroutine(CreateNewMap());
        }

        private IEnumerator CreateNewMap()
        {
            yield return StartCoroutine(CreateSeaFloor());
            CreateReef();
        }

        public void CreateReef()
        {
            GameObject map = Instantiate(mapPrefab, CalculateNextPosition(), Quaternion.identity);
            StartCoroutine(map.GetComponent<MapGenerator>().GenerateMap(this, mapSize, squareSize));
            maps.Add(map);
        }

        private IEnumerator CreateSeaFloor()
        {
            Vector3 seaFloorSize = new Vector3(mapSize.x * 2, mapSize.y / 2);
            GameObject map = Instantiate(seaFloorPrefab, CalculateNextPosition(), Quaternion.identity);
            yield return StartCoroutine(map.GetComponent<MapGenerator>().GenerateSeaFloor(this, seaFloorSize, squareSize));
            maps.Add(map);
        }

        private Vector3 CalculateNextPosition()
        {
            if (maps.Count > 0)
            {
                MapGenerator mapGenerator = maps[maps.Count - 1].GetComponent<MapGenerator>();

                Vector2 nextPosition = new Vector2(mapGenerator.transform.position.x + (mapGenerator.ExitLocation.x - mapGenerator.MapSize.x / 2) * squareSize,
                                                    mapGenerator.transform.position.y - (mapGenerator.MapSize.y / 2 + mapSize.y / 2 - 1) * squareSize);

                return new Vector3(nextPosition.x, nextPosition.y);
            }

            return startPosition;
        }
    }

}
