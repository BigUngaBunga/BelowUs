using UnityEngine;

namespace BelowUs
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject firePoint;
        [SerializeField] private GameObject bulletPrefab;

        private void Awake() => AdjustFirepoint();

        public void Shoot() => Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation, ReferenceManager.Singleton.BulletParent);

        /**
         * Dynamically adjusts firePoint based on bullet size
         */
        private void AdjustFirepoint()
        {
            var increase = bulletPrefab.GetComponent<CircleCollider2D>().radius / 2;
            var posX = firePoint.transform.localPosition.x;
            var posY = firePoint.transform.localPosition.y;

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

            firePoint.transform.localPosition = new Vector2(posX + changeX, posY + changeY);
        }
    }
}
