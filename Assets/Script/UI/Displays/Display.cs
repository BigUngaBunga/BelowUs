using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BelowUs
{
    public abstract class Display : MonoBehaviour
    {
        protected TMP_Text textDisplay;
        protected virtual void Start()
        {
            textDisplay = GetComponentInParent<TMP_Text>();
            InvokeRepeating(nameof(UpdateDisplay), 0.1f, 0.3f);
        }
        protected abstract void UpdateDisplay();
    }
}


