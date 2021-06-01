using System;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class SubmarineCannon : NetworkBehaviour
    {
        private readonly int submarineCannons = 4;

        private bool flipped;

        [SerializeField] private int restrictionLeft;
        [SerializeField] private int restrictionRight;

        [SerializeField] private int targetingOffset;
        [SerializeField] private int rotationOffset;
        
        [SerializeField] private int cannonId;
        [SerializeField] private float intensity;
        [SerializeField] private FloatVariable spotAngle;

        [SerializeField] private float fireRate;
        [SerializeField] private float ammunition;
        [SerializeField] private float reloadTimer = 3;
        private bool reloading;

        private float reloadSize;
        private float fireRateTimer;

        [SerializeField] private StationController cannonController;

        private SpriteRenderer spriteRenderer;
        private SubmarineMovement submarine;
        private Light spotlight;

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
            ammunition = reloadSize = 20;
            fireRate = fireRateTimer = 0.2f;


            spriteRenderer = GetComponent<SpriteRenderer>();
            submarine = GetComponentInParent<SubmarineMovement>();
            spotlight = GetComponentInChildren<Light>();
            spotlight.spotAngle = spotAngle.Value;
            InvokeRepeating(nameof(ToggleSpotlight), 0, 0.1f);

            if (cannonId < 0 || cannonId > submarineCannons - 1)
                Debug.LogError(logError + "has an incorrect cannonId!");
        }

        private void Update()
        {
            FlipCannon(); //TODO Flip when submarine flipbutton is pressed instead of checking every frame
            Targeting();
            Fire();
            Reload();
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
                    if (isServer)
                        RotateCannon(angleDeg);
                    else
                        CallRotateCannon(angleDeg);
                }
            }
        }

        [Command] private void CallRotateCannon(float angleDeg) => RotateCannon(angleDeg);

        [Server] private void RotateCannon(float angleDeg) => transform.rotation = Quaternion.Euler(0, 0, angleDeg + rotationOffset);

        private bool IsCannonActive() => cannonController.IsOccupied && NetworkClient.localPlayer == cannonController.StationPlayerController;

        private void ToggleSpotlight() => spotlight.intensity = IsCannonActive()? intensity : 0;
        
        private void Fire()
        {
            fireRate -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && IsCannonActive() && fireRate <= 0 && ammunition > 0 && !reloading)
            {
                NetworkBehaviour LocalPlayerNetworkBehaviour = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.LocalPlayerTag).GetComponent<NetworkBehaviour>();

                if (LocalPlayerNetworkBehaviour == isServer)
                    weapon.Shoot();
                else
                    weapon.CmdShoot();

                fireRate = fireRateTimer;
                ammunition--;
            }               
        }
        
        private void Reload()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                reloading = true;
            }
            if (reloading)
            {
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0 && IsCannonActive())
                {
                    ammunition = reloadSize;
                    reloadTimer = 3;
                    reloading = false;
                }
            }
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