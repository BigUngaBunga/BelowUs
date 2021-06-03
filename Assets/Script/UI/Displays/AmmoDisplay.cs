using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BelowUs
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ammoLabel, reloadingLabel;

        public void UpdateUI(int ammunitionRemaining, int totalAmmunition, bool isReloading)
        {
            ammoLabel.text = $"{ammunitionRemaining} / {totalAmmunition}";
            reloadingLabel.color = isReloading ? Color.red : Color.green;
        }
    }
}

