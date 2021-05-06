using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BelowUs
{
    public class DisplayControlsButton : MonoBehaviour
    {
        [SerializeReference] private GameObject controlUI;
        [SerializeReference] private TMP_Text buttonText;

        //private void Start() => buttonText = GetComponent<TMP_Text>();

        public void ChangeVisibility()
        {
            buttonText.text = controlUI.activeSelf ? "Show Controls" : "Hide Controls";
            controlUI.SetActive(!controlUI.activeSelf);
        }


    }
}

