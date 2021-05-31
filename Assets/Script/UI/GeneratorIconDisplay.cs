using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class GeneratorIconDisplay : MonoBehaviour
    {
        [SerializeReference] private ShipResource resource;
        [SerializeReference] private Image background;
        [SerializeField] private float riskZone;

        private void Start() => InvokeRepeating(nameof(UpdateColour), 0, 1);

        private void UpdateColour()
        {
            if (resource.CurrentValue != 0)
                background.color = resource.CurrentValue > riskZone ? Color.green : Color.yellow;
            else
                background.color = Color.red;
        }
    }
}


