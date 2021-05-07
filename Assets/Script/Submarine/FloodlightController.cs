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
        private float intensity;
        private SubmarineMovement submarineMovement;
        private SpriteRenderer spriteRenderer;
        private bool IsOccupied => floodlightController.StationPlayerController != null;
        private Light spotLight;
        private bool hasFlippedFloodlight;
        void Start()
        {
            intensity = 2;
            float updateTimer = 0.2f;
            submarineMovement = GetComponentInParent<SubmarineMovement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spotLight = GetComponentInChildren<Light>();
            InvokeRepeating(nameof(ToggleFloodlight), 0.1f, updateTimer);
        }

        void Update()
        {
            RotateFloodlight();
            //FlipFloodlight();
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

        private void ToggleFloodlight() => spotLight.intensity = IsOccupied ? intensity : 0;

        //private void FlipFloodlight()
        //{
        //    if (hasFlippedFloodlight != submarineMovement.IsFlipped)
        //    {
        //        hasFlippedFloodlight = submarineMovement.IsFlipped;
        //        spriteRenderer.flipX = hasFlippedFloodlight;
        //        transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        //    }
        //}
    }
}

