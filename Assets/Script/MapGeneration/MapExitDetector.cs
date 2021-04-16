using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class MapExitDetector : MonoBehaviour
    {
        public bool HasExitedRoom { get; private set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Collider activated");
            if (collision.gameObject.tag.Equals("Submarine"))
                HasExitedRoom = true;
        }

        public void CreateExitDetector(Vector2 exitPosition, Vector2 detectorSize)
        {
            BoxCollider2D currentDetector = gameObject.GetComponent<BoxCollider2D>();
            Destroy(currentDetector);

            BoxCollider2D exitDetector = gameObject.AddComponent<BoxCollider2D>();
            exitDetector.isTrigger = true;
            exitDetector.size = detectorSize;
            exitDetector.offset = -exitPosition;
            //Debug.Log(exitDetector.transform.position);
        }

        public void DeactivateCollider()
        {
            Destroy(this);
        }
    }
}

