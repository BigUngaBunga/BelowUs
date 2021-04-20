using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapHandler : MonoBehaviour
    {
        private List<GameObject> maps;
        private int squareSize;
        [SerializeReference] private GameObject mapPrefab;

        private void Start()
        {
            maps = new List<GameObject> ();
            squareSize = 2;
            CreateNewMap();
        }
        
        public void CreateNewMap()//Vector2 startPosition
        {
            Vector2 startPosition = CalculateNextPosition();
            Debug.Log(startPosition);
            GameObject map = Instantiate(mapPrefab, new Vector3(startPosition.x, startPosition.y), Quaternion.identity);
            map.GetComponent<MapGenerator>().GenerateMap(this, squareSize);
            maps.Add(map);
        }

        private Vector2 CalculateNextPosition()
        {
            if (maps.Count > 0)
            {
                MapGenerator mapGenerator = maps[maps.Count - 1].GetComponent<MapGenerator>();
                return new Vector2(mapGenerator.transform.position.x + (mapGenerator.ExitLocation.x - mapGenerator.MapSize.x / 2) * squareSize,
                                    mapGenerator.transform.position.y - (mapGenerator.MapSize.y - 1) * squareSize);
            }

            return Vector2.zero;
        }
    }

}
