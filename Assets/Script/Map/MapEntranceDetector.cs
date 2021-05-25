using UnityEngine;

namespace BelowUs
{
    public class MapEntranceDetector : MonoBehaviour
    {
        private bool hasEnteredRoom;
        private MapHandler mapHandler;
        private BoxCollider2D exitDetector;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(ReferenceManager.Singleton.SubmarineTag) && !hasEnteredRoom)
            {
                //StartCoroutine(mapHandler.GenerateNextMap());
                mapHandler.CommandGenerateNextMap();
                hasEnteredRoom = true;
                Destroy(exitDetector);
                Destroy(this);
            }
        }

        public void CreateEntranceDetector(int passagewayRadius, Vector2 mapSize, int squareSize, MapHandler mapHandler)
        {
            this.mapHandler = mapHandler;

            BoxCollider2D currentDetector = gameObject.GetComponent<BoxCollider2D>();
            Destroy(currentDetector);

            exitDetector = gameObject.AddComponent<BoxCollider2D>();
            exitDetector.isTrigger = true;
            exitDetector.size = new Vector2(passagewayRadius * 6, passagewayRadius * 2) * squareSize;
            exitDetector.offset = new Vector2(0, ((mapSize.y * squareSize) - exitDetector.size.y) / 2);
        }
    }
}

