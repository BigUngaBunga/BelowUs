using System.Collections;
using UnityEngine;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class BossRoomGenerator : ReefGenerator
    {
        [SerializeField] private GameObject boss;
        [SerializeField] private Vector2 position;
        
        public IEnumerator GenerateBossRoom(Vector2 mapSize, int squareSize, Random random)
        {
            this.random = random;
            position = transform.position;
            yield return StartCoroutine(GenerateNoiseMap(mapSize));

            yield return StartCoroutine(GetComponent<MeshGenerator>().GenerateMesh(noiseMap, squareSize, wallTile));
            SpawnBoss();            
        }

        private void SpawnBoss()
        {
            GameObject enemy = Instantiate(boss, position, Quaternion.identity);
            NetworkServer.Spawn(enemy, connectionToServer);
        }
    }
}

