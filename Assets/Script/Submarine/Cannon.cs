using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

namespace BelowUs
{
    public class Cannon : MonoBehaviour
    {
        private Vector3 mousePos, pos;
        private Submarine_Movement submarineMovement;
        private SpriteRenderer spriteRenderer;
        private float angleRad, angleDeg, offset, subRotation;
        private float minimumRotation, maximumRotation;
        private bool hasFlippedCannon;
        private new Light light;

        [SerializeField] private StationController cannonController;
        private bool IsOccupied => cannonController.StationPlayerController != null && ClientScene.localPlayer.gameObject == cannonController.StationPlayerController;

        private void Start()
        {
            offset = 90;
            minimumRotation = 7;
            maximumRotation = 54;
            submarineMovement = GetComponentInParent<Submarine_Movement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            light = GetComponentInChildren<Light>();
        }

        //TODO make controllable by only player in station

        //TODO change to new input system
        private void Update()
        {
            if (IsOccupied)
                light.intensity = 1;
            else
                light.intensity = 0;

            if(Camera.main != null)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                angleRad = Mathf.Atan2(mousePos.y - pos.y, mousePos.x - pos.x);
                if (angleDeg + minimumRotation <= maximumRotation + subRotation && angleDeg + minimumRotation >= -maximumRotation + subRotation)
                    transform.rotation = Quaternion.Euler(0, 0, angleDeg + 90);
            }

            pos = transform.position;
            subRotation = (float)(Mathf.Atan2(pos.y - transform.parent.position.y, pos.x - transform.parent.position.x) / Math.PI * 180) + minimumRotation;
            angleDeg = (float)(angleRad / Math.PI * 180);
            if (angleDeg < 0)
                angleDeg += 360;

            if (subRotation < 0)
                subRotation += 360;

            FlipCannon();
        }

        private void FlipCannon()
        {
            if (hasFlippedCannon != submarineMovement.IsFlipped)
            {
                hasFlippedCannon = submarineMovement.IsFlipped;
                spriteRenderer.flipX = hasFlippedCannon;
                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            }  
        }
    }

}
