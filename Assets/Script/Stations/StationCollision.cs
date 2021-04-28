using UnityEngine;

namespace BelowUs
{
    public class StationCollision : MonoBehaviour
    {
        private StationController controller;

        void Start() => controller = GetComponentInParent<StationController>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                collision.gameObject.GetComponent<PlayerCharacterController>().Station = controller;
        }
    }
}
