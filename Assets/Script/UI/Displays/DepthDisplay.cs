using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class DepthDisplay : Display
    {
        [SerializeReference] private Transform submarinePosition;
        private float initialDepth;

        protected override void Start()
        {
            base.Start();
            initialDepth = submarinePosition.position.y;
        }
        protected override void UpdateDisplay() => depthDisplay.text = ((int)(-submarinePosition.position.y + initialDepth)).ToString() + " m";
    }
}

