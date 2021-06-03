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

        [SerializeField] private float reloadTime;
        [SerializeField] private float reloadSize;
        [SerializeField] private float fireRateTimer;

        [SyncVar]  private float ammunition;
        [SyncVar] private bool reloading;
        [SyncVar] private float reloadTimer;
        private float fireRate;




        [SerializeField] private StationController cannonController;
        
        private AmmoDisplay ammoUI;


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
            ammunition = reloadSize;
            fireRate = fireRateTimer;
            reloadTimer = reloadTime;

            ammoUI = GameObject.Find("Game/UI/AmmoDisplay").GetComponent<AmmoDisplay>();
            UpdateAmmoUI();

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
            RepeatUpdateUI();
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
                    else if (NetworkClient.ready)
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
                    Shoot();
                else
                    CommandShoot();
                
                UpdateAmmoUI();
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
                    reloadTimer = reloadTime;
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

        public void RepeatUpdateUI()
        {
            if (IsCannonActive() && !IsInvoking(nameof(UpdateAmmoUI)))
            {
                float repeatTime = 0.2f;
                InvokeRepeating(nameof(UpdateAmmoUI), 0, repeatTime);
            }
            else if (!IsCannonActive() && IsInvoking(nameof(UpdateAmmoUI)))
                CancelInvoke(nameof(UpdateAmmoUI));
        }

        private void UpdateAmmoUI() => ammoUI.UpdateUI((int)ammunition, (int)reloadSize, reloading);

        [Server]
        private void Shoot()
        {
            weapon.Shoot();
            fireRate = fireRateTimer;
            ammunition--;
        }

        [Command]
        private void CommandShoot() => Shoot();
    }
}