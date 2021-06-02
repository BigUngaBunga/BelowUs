using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class ButtonDisplay : MonoBehaviour
    {
        private GameObject buttonUI;
        //private StationController stationCollider;
        private void Start()
        {
            buttonUI = GetComponent<GameObject>();
            buttonUI.SetActive(false);
        }

        public void ChangeButtonVisibility(bool isVisible) => buttonUI.SetActive(isVisible);
    }
}

