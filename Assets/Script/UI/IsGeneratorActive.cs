using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BelowUs
{
    public class IsGeneratorActive : MonoBehaviour
    {
        [SerializeReference] private string activeLabel;
        [SerializeReference] private string inactiveLabel;
        [SerializeReference] private ShipResource shipResource;
        private TMP_Text label;

        private void Start()
        {
            label = GetComponent<TMP_Text>();
            InvokeRepeating(nameof(UpdateLabel), 0.1f, 0.1f);
        }

        private void UpdateLabel()
        {
            if (shipResource.CurrentValue <= 0)
            {
                label.text = inactiveLabel;
                label.color = Color.red;
            }
            else
            {
                label.text = activeLabel;
                label.color = Color.green;
            }  
        }
    }
}

