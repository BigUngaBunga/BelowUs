using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class SceneController : NetworkBehaviour
    {
        [SerializeField] private ShipResource submarineHealth;

        // Start is called before the first frame update
        private void Start()
        {
            if (isServer)
                submarineHealth.EventResourceEmpty += SwitchScene;
        }

        [Server]
        private void SwitchScene() => NetworkManager.singleton.ServerChangeScene("Village");
    }
}