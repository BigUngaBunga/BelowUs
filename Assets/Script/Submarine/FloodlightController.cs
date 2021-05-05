using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class FloodlightController : MonoBehaviour
    {
        [SerializeField] private StationController floodlightController;
        [SerializeReference] private float intensity;
        [SerializeReference] private float updateTimer;
        private Submarine_Movement submarineMovement;
        private SpriteRenderer spriteRenderer;
        private bool IsOccupied => floodlightController.StationPlayerController != null;// && NetworkClient.localPlayer.gameObject == floodlightController.StationPlayerController;
        private Light spotLight;
        private bool hasFlippedFloodlight;
        void Start()
        {
            submarineMovement = GetComponentInParent<Submarine_Movement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spotLight = GetComponentInChildren<Light>();
            InvokeRepeating(nameof(ToggleFloodlight), 0.1f, updateTimer);
        }

        void Update()
        {
            RotateFloodlight();
            FlipFloodlight();
        }

        private void RotateFloodlight()
        {
            Vector2 mousePosition;
            float angle;

            if (IsOccupied)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle + 90);
            }
            else
                transform.rotation = transform.parent.rotation;
        }

        private void ToggleFloodlight()
        {
            if (IsOccupied)
                spotLight.intensity = intensity;
            else
                spotLight.intensity = 0;
        }

        private void FlipFloodlight()
        {
            if (hasFlippedFloodlight != submarineMovement.IsFlipped)
            {
                hasFlippedFloodlight = submarineMovement.IsFlipped;
                spriteRenderer.flipX = hasFlippedFloodlight;
                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }
}

