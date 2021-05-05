using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BelowUs
{
    public class GeneratorController : StationController
    {
        private Canvas generatorUI;

        private void Start()
        {
            generatorUI = GetComponentInChildren<Canvas>();
            generatorUI.gameObject.SetActive(false);
        }

        protected override void EnterStation(PlayerInput player)
        {
            base.EnterStation(player);
            generatorUI.gameObject.SetActive(true);
        }

        public override void Leave()
        {
            base.Leave();
            generatorUI.gameObject.SetActive(false);
        }
    }
}

