using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;

        private Transform firePoint;

        private void Awake()
        {
            transform.Find("Test");

            firePoint = transform.Find("FirePoint & Light");

            if (firePoint == null)
                firePoint = transform.Find("FirePoint");

            AdjustFirepoint();
        }

        public void Shoot() {

             NetworkServer.Spawn(Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, ReferenceManager.Singleton.BulletParent));
        } 

        /**
         * Dynamically adjusts firePoint based on bullet size
         */
        private void AdjustFirepoint()
        {
            var increase = bulletPrefab.GetComponent<CircleCollider2D>().radius / 2;
            var posX = firePoint.localPosition.x;
            var posY = firePoint.localPosition.y;

            var changeX = 0f;
            var changeY = 0f;

            if (posX > 0)
                changeX = increase;
            else if (posX < 0)
                changeX = -increase;

            if (posY > 0)
                changeY = increase;
            else if (posY < 0)
                changeY = -increase;

            firePoint.localPosition = new Vector2(posX + changeX, posY + changeY);
        }
    }
}
