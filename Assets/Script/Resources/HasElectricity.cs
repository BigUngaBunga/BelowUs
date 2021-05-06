using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class HasElectricity : MonoBehaviour
    {
        public bool IsPowered => electricity.CurrentValue > 0;
        private ShipResource electricity;

        private void Start() => electricity = GetComponent<ShipResource>();
    }
}

