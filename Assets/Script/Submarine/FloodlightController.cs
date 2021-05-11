using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class FloodlightController : NetworkBehaviour
    {
        [SerializeField] private StationController floodlightController;
        [SerializeField] private float intensity = 2;
        private SubmarineMovement submarineMovement;
        private SpriteRenderer spriteRenderer;
        private bool IsOccupied => floodlightController.StationPlayerController != null;
        private Light spotLight;
        void Start()
        {
            float updateTimer = 0.2f;
            submarineMovement = GetComponentInParent<SubmarineMovement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spotLight = GetComponentInChildren<Light>();

            if (isServer)
                InvokeRepeating(nameof(ToggleFloodlight), 0.1f, updateTimer);
        }

        void Update() => RotateFloodlight();

        private void RotateFloodlight()
        {
            if (IsOccupied && NetworkClient.localPlayer.gameObject == floodlightController.StationPlayerController)
            {
                Vector2 mousePosition;
                float angle;
                mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
                transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * angle) + 90);
            }
        }

        [Server]
        private void ToggleFloodlight() => spotLight.intensity = IsOccupied ? intensity : 0;
    }
}

