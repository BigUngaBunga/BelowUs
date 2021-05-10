using System;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class SubmarineCannon : MonoBehaviour
    {
        private readonly int submarineCannons = 4;

        private bool flipped;

        [SerializeField] private int restrictionLeft;
        [SerializeField] private int restrictionRight;

        [SerializeField] private int targetingOffset;
        [SerializeField] private int rotationOffset;
        
        [SerializeField] private int cannonId;

        private Vector3 startingRotation;
        private Vector3 lastKnownMousePos;

        private SpriteRenderer spriteRenderer;
        private SubmarineMovement submarine;
        private Light spotlight;
        private float intensity; 

        [SerializeField] private StationController cannonController;

        private Weapon weapon;

        private string logError;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();

            logError = gameObject.name + " in " + gameObject.transform.parent + " ";

            if (cannonController == null || weapon == null)
            {
                Debug.LogError(logError + "has no cannonController and/or weapon!");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            submarine = GetComponentInParent<SubmarineMovement>();
            spotlight = GetComponentInChildren<Light>();
            intensity = spotlight.intensity;
            InvokeRepeating(nameof(ToggleSpotlight), 0, 0.1f);
            startingRotation = transform.eulerAngles;

            if (cannonId < 0 || cannonId > submarineCannons - 1)
                Debug.LogError(logError + "has an incorrect cannonId!");
        }

        private void Update()
        {
            FlipCannon(); //TODO Flip when submarine flipbutton is pressed instead of checking every frame
            Targeting();
            Fire();
        }

        //TODO lyssna efter något event som aktiverar och avaktiverar denna metoden istället för att alltid köra den och kolla IsCannonActive varje gång
        private void Targeting()
        {
            if (IsCannonActive())
            {
                Vector3 pos = transform.position;
                Vector3 parentPos = transform.parent.position;

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                double subRotation = (Mathf.Atan2(pos.y - parentPos.y, pos.x - parentPos.x) / Math.PI * 180) + 7 + targetingOffset;
                float angleRad = Mathf.Atan2(mousePos.y - pos.y, mousePos.x - pos.x);
                float angleDeg = (float)(angleRad / Math.PI * 180) + targetingOffset;

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

        private bool IsCannonActive() => cannonController.StationPlayerController != null && NetworkClient.localPlayer.gameObject == cannonController.StationPlayerController;

        private void ToggleSpotlight() => spotlight.intensity = IsCannonActive()? intensity : 0;

        private void Fire()
        {
            if (Input.GetMouseButtonDown(0) && IsCannonActive())
                weapon.Shoot();
        }

        private void FlipCannon()
        {
            if (flipped != submarine.IsFlipped)
            {
                flipped = submarine.IsFlipped;
                spriteRenderer.flipX = flipped;
                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                int previousLeftRestrict = restrictionLeft;
                restrictionLeft = -restrictionRight;
                restrictionRight = -previousLeftRestrict;
            }
        }
    }
}