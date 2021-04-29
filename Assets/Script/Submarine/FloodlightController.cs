using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class FloodlightController : MonoBehaviour
    {
        [SerializeField] private StationController floodlightController;
        [SerializeReference] private float intensity, updateTimer;
        private bool IsOccupied => floodlightController.StationPlayerController != null && ClientScene.localPlayer.gameObject == floodlightController.StationPlayerController;
        private Light spotLight;
        void Start()
        {
            spotLight = GetComponentInChildren<Light>();
            InvokeRepeating("ToggleFloodlight", 0.1f, updateTimer);
        }

        void Update()
        {
        }

        private void RotateFloodlight()
        {
            if (IsOccupied)
            {

            }
        }

        private void ToggleFloodlight()
        {
            if (IsOccupied)
                spotLight.intensity = intensity;
            else
                spotLight.intensity = 0;
        }
    }
}

