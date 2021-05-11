using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyManager : NetworkBehaviour
{
    [SyncVar]
    private List<GameObject> EnemyList = new List<GameObject>();
    [SerializeField] private GameObject SkeletonPrefab;
    private void Start()
    {
        //SpawnEnemy(new Vector3(0, 0, 0));
        //SpawnEnemy(new Vector3(0, 100, 0));
        //SpawnEnemy(new Vector3(0, 200, 0));
    }

    private void Update()
    {        
    }

    private void SpawnEnemy(Vector3 position)
    {
        var enemy = NetworkManager.Instantiate(SkeletonPrefab, position, Quaternion.identity);
        EnemyList.Add(enemy);
        NetworkServer.Spawn(enemy);
    }
}
