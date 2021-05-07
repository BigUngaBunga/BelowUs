using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class LightsWhenPowered : MonoBehaviour
    {
        //Place in the same component as where there is a lightsource
        private HasElectricity hasElectricity;
        private new Light light;
        [SerializeField] bool invertToggle = false;

        private void Start()
        {
            hasElectricity = GameObject.Find("ElectricityGeneration").GetComponent<HasElectricity>();
            light = GetComponent<Light>();
            InvokeRepeating(nameof(ToggleLight), 0, 0.25f);
        }

        private void ToggleLight() => light.enabled = invertToggle? !hasElectricity.IsPowered : hasElectricity.IsPowered;
    }
}

