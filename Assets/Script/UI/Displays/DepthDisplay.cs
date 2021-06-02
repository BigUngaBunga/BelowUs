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
            initialDepth = 200;
            //initialDepth = submarinePosition.position.y;
        }
        protected override void UpdateDisplay() => textDisplay.text = ((int)(-submarinePosition.position.y + initialDepth)).ToString() + " m";
    }
}

