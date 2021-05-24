using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class GeneratorController : NetworkBehaviour
    {
        private Canvas generatorUI;

        [SerializeField] [SyncVar] protected NetworkIdentity stationPlayerController = null;
        public bool IsOccupied => stationPlayerController != null;

        protected virtual void Start()
        {
            generatorUI = GetComponentInChildren<Canvas>(true);

            generatorUI.gameObject.SetActive(false);
        }

        [Command(requiresAuthority = false)] private void SetStationPlayerControllerCMD(NetworkIdentity player) => SetStationPlayerController(player);

        [Server] private void SetStationPlayerController(NetworkIdentity player) => stationPlayerController = player;

        
        public void Enter(NetworkIdentity player)
        {
            SetUIActive(true);

            if (isServer)
                SetStationPlayerController(player);
            else
                SetStationPlayerControllerCMD(player);
        }

        public void Leave()
        {
            SetUIActive(false);

            if (isServer)
                SetStationPlayerController(null);
            else
                SetStationPlayerControllerCMD(null);
        }

        protected virtual void SetUIActive(bool setActive) => generatorUI.gameObject.SetActive(setActive);
    }
}

