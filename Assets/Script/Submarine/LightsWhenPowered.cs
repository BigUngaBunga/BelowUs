using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class LightsWhenPowered : MonoBehaviour
    {
        //Place in the same component as where there is a lightsource
        private new Light light;
        [SerializeField] bool invertToggle = false;
        [SerializeReference] private ShipResource electricity;
        public bool IsPowered => electricity.CurrentValue > 0;
        


        private void Start()
        {
            if (electricity == null)
                electricity = GameObject.Find("Game/Ship/Resources/ElectricityGeneration").GetComponent<ShipResource>();

            light = GetComponent<Light>();
            InvokeRepeating(nameof(ToggleLight), 0, 0.25f);
        }

        private void ToggleLight() => light.enabled = invertToggle? !IsPowered : IsPowered;
    }
}

