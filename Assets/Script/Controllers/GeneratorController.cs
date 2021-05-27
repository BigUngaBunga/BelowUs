using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class GeneratorController : BaseStationController
    {
        private Canvas generatorUI;

        protected virtual void Start()
        {
            generatorUI = GetComponentInChildren<Canvas>(true);

            generatorUI.gameObject.SetActive(false);
        }

        [Command(requiresAuthority = false)] private void SetStationPlayerControllerCMD(NetworkIdentity player) => SetStationPlayerController(player);

        [Server] private void SetStationPlayerController(NetworkIdentity player) => stationPlayerController = player;

        
        public void Enter(NetworkIdentity player)
        {
            Debug.Log("Activate UI");
            SetUIActive(true);

            if (isServer)
                SetStationPlayerController(player);
            else
                SetStationPlayerControllerCMD(player);
        }

        public void Leave()
        {
            Debug.Log("Deactivate UI");
            SetUIActive(false);

            if (isServer)
                SetStationPlayerController(null);
            else
                SetStationPlayerControllerCMD(null);
        }

        protected virtual void SetUIActive(bool setActive) => generatorUI.gameObject.SetActive(setActive);
    }
}

