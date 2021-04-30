using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class DepthDisplay : Display
    {
        [SerializeReference] private Transform submarinePosition;
        protected override void UpdateDisplay() => depthDisplay.text = ((int)-submarinePosition.position.y).ToString() + " m";
    }
}

