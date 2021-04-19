using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapHandler : MonoBehaviour
    {
        private static List<GameObject> maps;
        [SerializeReference] private GameObject map;
        static GameObject mappi;

        private void Start()
        {
            mappi = map;
            maps = new List<GameObject> ();
            CreateNewMap(Vector2.zero);
        }

        public static void CreateNewMap(Vector2 startPosition)
        {
            maps.Add(Instantiate(mappi, new Vector3(startPosition.x, startPosition.y), Quaternion.identity));
        } 
    }

}
