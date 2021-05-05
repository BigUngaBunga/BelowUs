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

        public override void Enter(GameObject player)
        {
            base.Enter(player);
            generatorUI.gameObject.SetActive(true);
        }

        public override void Leave()
        {
            base.Leave();
            generatorUI.gameObject.SetActive(false);
        }
    }
}

