using System;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BaseCannon : MonoBehaviour
    {
        protected bool flipped;

        [SerializeField] protected float leftRestrict, rightRestrict, whichCannon;

        private Vector3 startingRotation;
        protected Vector3 lastKnownMousePos;

        private SpriteRenderer spriteRenderer;
        private SubmarineMovement submarine;
        private Light spotlight;
        private float intensity; 

        [SerializeReference] private GameObject bullet;
        [SerializeField] private StationController cannonController;

        protected virtual void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            submarine = GetComponentInParent<SubmarineMovement>();
            spotlight = GetComponentInChildren<Light>();
            intensity = spotlight.intensity;
            InvokeRepeating(nameof(ToggleSpotlight), 0, 0.1f);
            startingRotation = transform.eulerAngles;
        }

        protected virtual void Update()
        {
            FlipCannon();
            ActiveCannon();
        }

        protected void Targeting(Vector3 pos, float offset, float rotationOffset, float restrictionLeft, float restrictionRight)
        {
            if (IsCannonActive())
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float subRotation = (float)(Mathf.Atan2(pos.y - transform.parent.position.y, pos.x - transform.parent.position.x) / Math.PI * 180) + 7 + offset;
                float angleRad = Mathf.Atan2(mousePos.y - pos.y, mousePos.x - pos.x);
                float angleDeg = (float)(angleRad / Math.PI * 180) + offset;

                if (angleDeg < 0)
                    angleDeg += 360;

                if (subRotation < 0)
                    subRotation += 360;

                if (angleDeg + 7 <= restrictionLeft + subRotation && angleDeg + 7 >= restrictionRight + subRotation)
                {
                    lastKnownMousePos = mousePos;
                    transform.rotation = Quaternion.Euler(0, 0, angleDeg + rotationOffset);
                }
            }
        }

            
        private bool IsCannonActive() => NetworkClient.localPlayer != null && cannonController != null && NetworkClient.localPlayer.gameObject == cannonController.StationPlayerController;

        protected void ActiveCannon()
        {
            if (IsCannonActive())
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
            else
                transform.eulerAngles = transform.parent.eulerAngles - startingRotation;
        }

        private void ToggleSpotlight() => spotlight.intensity = IsCannonActive()? intensity : 0;

        protected void Fire()
        {
            if (Input.GetMouseButtonDown(0) && IsCannonActive())
                Instantiate(bullet, transform.position, transform.rotation);
        }

        private void FlipCannon()
        {
            if (flipped != submarine.IsFlipped)
            {
                flipped = submarine.IsFlipped;
                spriteRenderer.flipX = flipped;
                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                float previousLeftRestrict = leftRestrict;
                leftRestrict = -rightRestrict;
                rightRestrict = -previousLeftRestrict;

            }
        }
    }
}