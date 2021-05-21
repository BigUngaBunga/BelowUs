using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class FloodlightController : NetworkBehaviour
    {
        [SerializeField] private StationController floodlightController;
        [SerializeField] private FloatVariable spotAngle;
        [SerializeField] private float intensity = 2;
        private SubmarineMovement submarineMovement;
        private SpriteRenderer spriteRenderer;
        private bool IsOccupied => floodlightController.IsOccupied;
        private Light spotLight;
        void Start()
        {
            float updateTimer = 0.2f;
            submarineMovement = GetComponentInParent<SubmarineMovement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spotLight = GetComponentInChildren<Light>();
            spotLight.spotAngle = spotAngle.Value;

            if (isServer)
                InvokeRepeating(nameof(ToggleFloodlight), 0.1f, updateTimer);
        }

        void Update()
        {
            if (IsOccupied && NetworkClient.localPlayer == floodlightController.StationPlayerController)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
                Quaternion rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * angle) + 90);

                if (isServer)
                    RotateFloodlight(rotation);
                else
                    CommandRotateFloodlight(rotation);
            }
        }

        [Command] 
        private void CommandRotateFloodlight(Quaternion rotation) => RotateFloodlight(rotation);

        [Server] 
        private void RotateFloodlight(Quaternion rotation) => transform.rotation = rotation;

        [ClientRpc]
        private void ToggleFloodlight() => spotLight.intensity = IsOccupied ? intensity : 0;
    }
}

