using UnityEngine;
using TMPro;

namespace BelowUs
{
    public class GoldDisplay : Display
    {
        [SerializeReference] private Transform gold;
        protected override void UpdateDisplay() => depthDisplay.text = ((int)gold.position.y).ToString();
    }
}

