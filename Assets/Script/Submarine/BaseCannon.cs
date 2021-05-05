using System;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BaseCannon : MonoBehaviour
    {
        protected bool flipped;

        protected float leftRestrict, rightRestrict, whichCannon;

        private float angleDeg, angleRad, subRotation;

        private Vector3 mousePos;
        protected Vector3 lastKnownMousePos;

        public GameObject bullet;

        private bool isCannonActive => IsCannonActive();

        [SerializeField] private StationController cannonController;

        protected void Targeting(Vector3 pos, float offset, float rotationOffset, float res1, float res2)
        {
            if (isCannonActive)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                subRotation = (float)(Mathf.Atan2(pos.y - transform.parent.position.y, pos.x - transform.parent.position.x) / Math.PI * 180) + 7 + offset;

                angleRad = Mathf.Atan2(mousePos.y - pos.y, mousePos.x - pos.x);

                angleDeg = (float)(angleRad / Math.PI * 180) + offset;

                if (angleDeg < 0)
                    angleDeg += 360;

                if (subRotation < 0)
                    subRotation += 360;

                if (angleDeg + 7 <= res1 + subRotation && angleDeg + 7 >= res2 + subRotation)
                {
                    lastKnownMousePos = mousePos;
                    transform.rotation = Quaternion.Euler(0, 0, angleDeg + rotationOffset);
                }
            }
        }

        private bool IsCannonActive()
        {
            if (NetworkClient.localPlayer != null && cannonController != null)
                return NetworkClient.localPlayer.gameObject == cannonController.StationPlayerController;
            
            return false;
        }

        protected void ActiveCannon()
        {
            if (isCannonActive)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    whichCannon = 1;

                if (Input.GetKeyDown(KeyCode.Alpha2))
                    whichCannon = 2;

                if (Input.GetKeyDown(KeyCode.Alpha3))
                    whichCannon = 3;

                if (Input.GetKeyDown(KeyCode.Alpha4))
                    whichCannon = 4;
            }
        }

        protected void Fire()
        {
            if (Input.GetMouseButtonDown(0) && isCannonActive)
                Instantiate(bullet, transform.position, Quaternion.LookRotation(lastKnownMousePos - transform.position).normalized);
        }
    }
}