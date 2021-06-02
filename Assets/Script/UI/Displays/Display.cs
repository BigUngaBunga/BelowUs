using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine;

namespace BelowUs
{
    public abstract class Display : MonoBehaviour
    {
        protected TMP_Text textDisplay;
        //[Server]
        protected virtual void Start()
        {
            textDisplay = GetComponentInParent<TMP_Text>();
            InvokeRepeating(nameof(UpdateDisplay), 0.1f, 0.3f);
        }

        //Empty virtual method due to ClientRpc not working with abstract methods
        //[ClientRpc]
        protected virtual void UpdateDisplay(){}
    }
}


