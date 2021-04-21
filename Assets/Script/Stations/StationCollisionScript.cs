using UnityEngine;

namespace BelowUs
{
    public class StationCollisionScript : MonoBehaviour
    {
        private Station station;

        private void Awake()
        {
            station = GetComponentInParent<Station>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            station.CheckCollision(collision);
        }
    }

}
