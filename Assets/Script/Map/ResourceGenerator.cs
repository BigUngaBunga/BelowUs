using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Mirror;
using Random = System.Random;

namespace BelowUs
{
    public class ResourceGenerator : NetworkBehaviour
    {
        private enum ResourceType
        {
            Gold, Scrap
        }

        [SerializeField] int numberOfResourcesToGenerate;
        [SerializeReference] GameObject goldPrefab, scrapPrefab;

        [SerializeField] [MustBeAssigned] private Transform goldParent;
        [SerializeField] [MustBeAssigned] private Transform scrapParent;

        private Random random;
        private List<Vector2> resourcePositions;
        private int[,] map;
        private int openTile, squareSize;

        public IEnumerator GenerateResources(Random random, int[,] map, int squareSize, int openTile)
        {
            yield return CorutineUtilities.Wait(0.01f, "Started resource generation");
            goldParent = GameObject.Find("Gold").transform;
            scrapParent = GameObject.Find("Scrap").transform;

            this.random = random;
            this.map = map;
            this.openTile = openTile;
            this.squareSize = squareSize;
            resourcePositions = new List<Vector2>();

            RandomizeResourcePlacements();
            yield return CorutineUtilities.Wait(0.01f, "Randomized resource positions");

            if (GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag).GetComponent<NetworkBehaviour>().isServer)
                foreach (Vector2 position in resourcePositions)
                    GenerateResource(position);
            yield return CorutineUtilities.Wait(0.01f, "Generated resources");
        }

        private void RandomizeResourcePlacements()
        {
            int halfMapWidth = map.GetLength(0) / 2 ;
            int halfMapHeight = map.GetLength(1) / 2;

            List<Vector2> openPositions = new List<Vector2>();
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    if (map[x, y] == openTile && SurroundedByOpenTiles(x, y))
                    {
                        Vector2 position;
                        position.x = ((x - halfMapWidth) * squareSize) + transform.position.x;
                        position.y = ((y - halfMapHeight) * squareSize) + transform.position.y;
                        openPositions.Add(position);
                    }
                        

            for (int i = 0; i < numberOfResourcesToGenerate; i++)
            {
                int randomIndex = random.Next(openPositions.Count - 1);
                resourcePositions.Add(openPositions[randomIndex]);
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
        private void GenerateResource(Vector2 position)
        {
            ResourceType resourceToInstantiate = PickWeightedResourceType();

            GameObject objectToInstantiate = resourceToInstantiate switch
            {
                ResourceType.Gold => goldPrefab,
                _ => scrapPrefab,
            };

            Transform parent = resourceToInstantiate == ResourceType.Gold ? goldParent : scrapParent;
            GameObject resource = Instantiate(objectToInstantiate, position, Quaternion.identity, parent);
            NetworkServer.Spawn(resource);
        }

        private ResourceType PickWeightedResourceType()
        {
            Dictionary<ResourceType, int> resourceTypes = new Dictionary<ResourceType, int>();
            int totalWeight = 0;

            resourceTypes.Add(ResourceType.Gold, 20);
            resourceTypes.Add(ResourceType.Scrap, 1);

            foreach (int weight in resourceTypes.Values)
                totalWeight += weight;

            int randomNumber = random.Next(totalWeight);
            foreach (KeyValuePair<ResourceType, int> weightedResourceTypes in resourceTypes)
            {
                if (weightedResourceTypes.Value > randomNumber)
                    return weightedResourceTypes.Key;
                else
                    randomNumber -= weightedResourceTypes.Value;
            }

            return ResourceType.Gold;
        }
    }
}

