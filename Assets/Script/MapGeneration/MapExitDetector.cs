using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapExitDetector : MonoBehaviour
    {
        public bool HasExitedRoom { get; private set; }
        private Vector2 exitPosition;
        private Vector2 mapSize;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Collider activated");
            if (!HasExitedRoom)
            {
                MapHandler.CreateNewMap(new Vector2(exitPosition.x - mapSize.x / 2, -mapSize.y + transform.position.y));
                HasExitedRoom = true;
            }
            
            if (collision.gameObject.tag.Equals("Submarine"))
                HasExitedRoom = true;
        }

        public void CreateExitDetector(Vector2 exitPosition, int passagewayRadius, Vector2 mapSize)
        {
            this.exitPosition = exitPosition;
            this.mapSize = mapSize;

            BoxCollider2D currentDetector = gameObject.GetComponent<BoxCollider2D>();
            Destroy(currentDetector);

            BoxCollider2D exitDetector = gameObject.AddComponent<BoxCollider2D>();
            exitDetector.isTrigger = true;
            exitDetector.size = new Vector2(passagewayRadius * 4, passagewayRadius * 2);
            exitDetector.offset = new Vector2(exitPosition.x - mapSize.x / 2, (-mapSize.y + exitDetector.size.y) / 2);
        }

        public void DeactivateCollider()
        {
            Destroy(this);
        }
    }
}

