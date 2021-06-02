using UnityEngine;
using Mirror;
using TMPro;

namespace BelowUs
{
    public class GoldDisplay : Display
    {
        [SerializeReference] private ShipResource gold;
        [SerializeReference] private FloatVariable goldTotal;
        [SerializeField] private bool displayShipResource;
        
        protected override void UpdateDisplay() => textDisplay.text = (displayShipResource ? (int)gold.CurrentValue : (int)goldTotal.Value).ToString();

    }
}

