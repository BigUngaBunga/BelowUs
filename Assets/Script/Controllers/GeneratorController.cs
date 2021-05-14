using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class GeneratorController : StationController
    {
        private Canvas generatorUI;

        private void Start()
        {
            generatorUI = GetComponentInChildren<Canvas>(true);

            generatorUI.gameObject.SetActive(false);
        }

        public override void Enter(NetworkIdentity player)
        {
            base.Enter(player);

            generatorUI.gameObject.SetActive(true);
        }

        public override void Leave()
        {
            base.Leave();
            generatorUI.gameObject.SetActive(false);
        }

        protected override void ChangeCameraToTag(string tag)
        {
            // Method intentionally left empty.
        }
    }
}

