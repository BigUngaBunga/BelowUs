using System.Collections;
using System.Collections.Generic;
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

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));
            SpawnBoss();            
        }  
        
        private void SpawnBoss()
        {
            GameObject enemy = Instantiate(boss, position, Quaternion.identity);
            NetworkServer.Spawn(enemy, connectionToServer);
        }
    }
}

