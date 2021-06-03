using Mirror;
using MyBox;
using UnityEngine;

namespace BelowUs
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] [MustBeAssigned] private GameObject bulletPrefab;

        //Todo bullet parent spawns all bullets at 0,0,0. Must be insstaniated right (world position)
        [SerializeField] [MustBeAssigned] private Transform bulletParent;

        private Transform firePoint;

        private void Awake()
        {
            transform.Find("Test");
            firePoint = transform.Find("FirePoint");

            if (bulletParent == null)
                bulletParent = GameObject.Find("Bullets").transform;

            AdjustFirepoint();
        }

        [Server]
        public void Shoot() {
            GameObject bulletClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, bulletParent);
            NetworkServer.Spawn(bulletClone);
        }

        [Command]
        public void CmdShoot() => Shoot();
        //{
        //    GameObject bulletClone = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, bulletParent);
        //    NetworkServer.Spawn(bulletClone);
        //}

        /**
         * Dynamically adjusts firePoint based on bullet size
         */
        private void AdjustFirepoint()
        {
            float posX = firePoint.localPosition.x;
            float posY = firePoint.localPosition.y;
            float changeX = 0f;
            float changeY = 0f;
            
            Vector2 increase = bulletPrefab.GetComponent<CapsuleCollider2D>().size / 2;
            
            if (posX > 0)
                changeX = increase.x;
            else if (posX < 0)
                changeX = -increase.x;

            if (posY > 0)
                changeY = increase.y;
            else if (posY < 0)
                changeY = -increase.y;
            firePoint.localPosition = new Vector3(posX + changeX, posY + changeY, -1);
        }
    }
}
