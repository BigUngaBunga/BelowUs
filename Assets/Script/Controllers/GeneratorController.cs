using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class GeneratorController : NetworkBehaviour
    {
        private Canvas generatorUI;

        [SerializeField] [SyncVar] private NetworkIdentity stationPlayerController = null;
        [SerializeField] public bool IsOccupied => stationPlayerController != null;

        private void Start()
        {
            generatorUI = GetComponentInChildren<Canvas>(true);

            generatorUI.gameObject.SetActive(false);
        }

        [Command(requiresAuthority = false)] private void SetStationPlayerControllerCMD(NetworkIdentity player) => SetStationPlayerController(player);

        [Server] private void SetStationPlayerController(NetworkIdentity player) => stationPlayerController = player;

        
        public void Enter(NetworkIdentity player)
        {
            generatorUI.gameObject.SetActive(true);

            if (isServer)
                SetStationPlayerController(player);
            else
                SetStationPlayerControllerCMD(player);
        }

        public void Leave()
        {
            generatorUI.gameObject.SetActive(false);

            if (isServer)
                SetStationPlayerController(null);
            else
                SetStationPlayerControllerCMD(null);
        }
    }
}

