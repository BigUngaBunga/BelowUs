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

        public int[,] Map { private get; set; }
        public int OpenTile { private get; set; }
        public int SquareSize { private get; set; }

        public IEnumerator GenerateEnemies(Random random)
        {
            yield return CorutineUtilities.Wait(0.01f, "Started enemy generation");

            //Todo parentmap spawns all Enemies at 0,0,0. Must be insstaniated right (world position)
            parentMap = GameObject.Find("Enemies").transform;

            this.random = random;
            enemyPositions = new List<Vector2>();

            enemyTypes.Add(EnemyType.Booba, spawnRateBobba);
            enemyTypes.Add(EnemyType.Clonker, spawnRateClonker);
            enemyTypes.Add(EnemyType.Popper, spawnRatePopper);
            enemyTypes.Add(EnemyType.Spook, spawnRateSpook);

            RandomizeEnemyPlacements();
            yield return CorutineUtilities.Wait(0.01f, "Randomized enemy positions");

            if (GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag).GetComponent<NetworkBehaviour>().isServer)
                foreach (Vector2 position in enemyPositions)
                    GenerateEnemies(position);
            yield return CorutineUtilities.Wait(0.01f, "Generated enemies");
        }

        private void RandomizeEnemyPlacements()
        {
            int halfMapWidth = Map.GetLength(0) / 2;
            int halfMapHeight = Map.GetLength(1) / 2;

            List<Vector2> openPositions = new List<Vector2>();
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                    if (Map[x, y] == OpenTile && SurroundedByOpenTiles(x, y))
                    {
                        Vector2 position;
                        position.x = (x - halfMapWidth) * SquareSize + transform.position.x;
                        position.y = (y - halfMapHeight) * SquareSize + transform.position.y;
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
            int stopX = Math.Min(xPosition + 1, Map.GetLength(0) - 1);
            int stopY = Math.Min(yPosition + 1, Map.GetLength(1) - 1);

            for (int x = startX; x < stopX; x++)
                for (int y = startY; y < stopY; y++)
                    if (Map[x, y] != OpenTile)
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
                objectToInstantiate = Instantiate(objectToInstantiate, position, Quaternion.identity);
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