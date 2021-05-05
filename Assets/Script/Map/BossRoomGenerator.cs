using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class BossRoomGenerator : ReefGenerator
    {
        public IEnumerator GenerateBossRoom(Vector2 mapSize, int squareSize)
        {
            yield return StartCoroutine(GenerateNoiseMap(mapSize));

            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
            yield return StartCoroutine(meshGenerator.GenerateMesh(noiseMap, squareSize, wallTile));
        }
    }
}

