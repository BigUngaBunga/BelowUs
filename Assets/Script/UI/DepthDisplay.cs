using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public class DepthDisplay : MonoBehaviour
    {
        [SerializeReference] private Transform submarinePosition;
        private TMP_Text depthDisplay;
        void Start()
        {
            depthDisplay = GetComponentInParent<TMP_Text>();
            InvokeRepeating(nameof(UpdateDisplay), 0.1f, 0.3f);
        }
        private void UpdateDisplay() => depthDisplay.text = ((int)-submarinePosition.position.y).ToString() + " m";
    }
}

