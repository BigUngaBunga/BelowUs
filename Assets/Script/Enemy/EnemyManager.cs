using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyManager : NetworkBehaviour
{
    [SyncVar]
    List<GameObject> EnemyList = new List<GameObject>();
    public GameObject SkeletonPrefab;
    void Start()
    {
        SpawnEnemy(new Vector3(0, 0, 0));
        SpawnEnemy(new Vector3(0, 100, 0));
        SpawnEnemy(new Vector3(0, 200, 0));
    }

    void Update()
    {
        foreach(GameObject enemy in EnemyList)
        {
        }
    }

    void SpawnEnemy(Vector3 position)
    {
        var enemy = NetworkManager.Instantiate(SkeletonPrefab, position, Quaternion.identity);
        EnemyList.Add(enemy);
        NetworkServer.Spawn(enemy);
    }
}
