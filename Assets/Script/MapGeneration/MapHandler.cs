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
            squareSize = 1;
            CreateNewMap(Vector2.zero);
        }

        public void CreateNewMap(Vector2 startPosition)
        {
            GameObject map = Instantiate(mapPrefab, new Vector3(startPosition.x, startPosition.y), Quaternion.identity);
            map.GetComponent<MapGenerator>().GenerateMap(this, squareSize);
            maps.Add(map);
        } 
    }

}
