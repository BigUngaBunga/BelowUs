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

        public bool ControlViewIsActive => controlUI.activeSelf;

        public void ChangeVisibility()
        {
            buttonText.text = controlUI.activeSelf ? "Show Controls" : "Hide Controls";
            controlUI.SetActive(!controlUI.activeSelf);
        }
    }
}

