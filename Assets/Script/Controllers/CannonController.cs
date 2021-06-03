using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class CannonController : StationController
    {
        [SerializeField] private GameObject ammoUI;

        public void ActivateUI(bool setActive) => ammoUI.SetActive(setActive);
    }
}


