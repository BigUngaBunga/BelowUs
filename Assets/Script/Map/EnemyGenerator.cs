using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class EnemyGenerator : NetworkBehaviour
    {
        
        private enum EnemyType
        {
            Booba,
            Spook,
            Clonker,
            Popper
        }

        [SerializeField][Min(5)] int numberOfEnemiesToGenerate;
        [SerializeReference] GameObject bobbaPrefab, spookPrefab, clonkerPrefab, popperPrefab;
        private Transform parentMap;

        private readonly Dictionary<EnemyType, int> enemyTypes = new Dictionary<EnemyType, int>();
        [SerializeField][Min (1)] private int spawnRateBobba, spawnRateSpook, spawnRateClonker, spawnRatePopper;


        private Random random;

        private List<Vector2> enemyPositions;
        //positioner

        private int[,] map;
        private int openTile, squareSize;

        public IEnumerator GenerateEnemies(Random random, int[,] map, int squareSize, int openTile)
        {
            parentMap = GameObject.Find("Enemies").transform;

            this.random = random;
            this.map = map;
            this.openTile = openTile;
            this.squareSize = squareSize;
            enemyPositions = new List<Vector2>();

            enemyTypes.Add(EnemyType.Booba, spawnRateBobba);
            enemyTypes.Add(EnemyType.Clonker, spawnRateClonker);
            enemyTypes.Add(EnemyType.Popper, spawnRatePopper);
            enemyTypes.Add(EnemyType.Spook, spawnRateSpook);


            yield return CorutineUtilities.Wait(0.01f, "Started resource generation");
            RandomizeEnemyPlacements();
            yield return CorutineUtilities.Wait(0.01f, "Randomized resource positions");

            if (GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag).GetComponent<NetworkBehaviour>().isServer)
                foreach (Vector2 position in enemyPositions)
                    GenerateEnemies(position);
            yield return CorutineUtilities.Wait(0.01f, "Generated resources");
        }

        private void RandomizeEnemyPlacements()
        {
            int halfMapWidth = map.GetLength(0) / 2;
            int halfMapHeight = map.GetLength(1) / 2;

            List<Vector2> openPositions = new List<Vector2>();
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    if (map[x, y] == openTile && SurroundedByOpenTiles(x, y))
                    {
                        Vector2 position;
                        position.x = (x - halfMapWidth) * squareSize + transform.position.x;
                        position.y = (y - halfMapHeight) * squareSize + transform.position.y;
                        openPositions.Add(position);
                    }


            for (int i = 0; i < numberOfEnemiesToGenerate; i++)
            {
                int randomIndex = random.Next(openPositions.Count - 1);
                enemyPositions.Add(openPositions[randomIndex]);
                openPositions.RemoveAt(randomIndex);
            }
        }

        private bool SurroundedByOpenTiles(int xPosition, int yPosition)
        {
            int startX = Math.Max(xPosition - 1, 0);
            int startY = Math.Max(yPosition - 1, 0);
            int stopX = Math.Min(xPosition + 1, map.GetLength(0) - 1);
            int stopY = Math.Min(yPosition + 1, map.GetLength(1) - 1);

            for (int x = startX; x < stopX; x++)
                for (int y = startY; y < stopY; y++)
                    if (map[x, y] != openTile)
                        return false;

            return true;
        }

        [Server]
        private void GenerateEnemies(Vector2 position)
        {
            GameObject objectToInstantiate = PickWeightedEnemyType() switch
            {
                EnemyType.Booba => bobbaPrefab,
                EnemyType.Spook => spookPrefab,
                EnemyType.Clonker => clonkerPrefab,
                EnemyType.Popper => popperPrefab,
                _ => bobbaPrefab,
            };
            objectToInstantiate = Instantiate(objectToInstantiate, position, Quaternion.identity, parentMap);
            NetworkServer.Spawn(objectToInstantiate);
        }

        private EnemyType PickWeightedEnemyType()
        {
            int totalWeight = 0;            

            foreach (int weight in enemyTypes.Values)
                totalWeight += weight;

            int randomNumber = random.Next(totalWeight);
            foreach (KeyValuePair<EnemyType, int> weightedEnemyTypes in enemyTypes)
            {
                if (weightedEnemyTypes.Value > randomNumber)
                    return weightedEnemyTypes.Key;
                else
                    randomNumber -= weightedEnemyTypes.Value;
            }
             return EnemyType.Booba;         
        }
    }
}