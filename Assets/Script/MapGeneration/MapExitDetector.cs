using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapExitDetector : MonoBehaviour
    {
        private bool hasExitedRoom;
        private MapHandler mapHandler;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Submarine") && !hasExitedRoom)
            {
                mapHandler.CreateNewMap();
                hasExitedRoom = true;
            }
        }

        public void CreateExitDetector(Vector2 exitPosition, int passagewayRadius, Vector2 mapSize, int squareSize, MapHandler mapHandler)
        {
            this.mapHandler = mapHandler;

            BoxCollider2D currentDetector = gameObject.GetComponent<BoxCollider2D>();
            Destroy(currentDetector);

            BoxCollider2D exitDetector = gameObject.AddComponent<BoxCollider2D>();
            exitDetector.isTrigger = true;
            exitDetector.size = new Vector2(passagewayRadius * 6, passagewayRadius * 2) * squareSize;
            exitDetector.offset = new Vector2((exitPosition.x - mapSize.x / 2) * squareSize, (-mapSize.y * squareSize + exitDetector.size.y) / 2);
        }
    }
}

