using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace BelowUs
{
    public class SceneController : NetworkBehaviour
    {
        [SerializeField] private ShipResource submarineHealth;
        [SerializeField] private ShipResource submarineOxygen;

        // Start is called before the first frame update
        private void Start()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (isServer && currentSceneName == "Game")
            {
                submarineHealth.EventResourceEmpty += SwitchToVillageScene;
                submarineOxygen.EventResourceEmpty += SwitchToVillageScene;
            }
                
        }

        [Server]
        private void SwitchToVillageScene() => NetworkManager.singleton.ServerChangeScene("Village");

        [Command(requiresAuthority = false)]
        public void CommandSwitchToGameScene() => SwitchToGameScene();

        [Server]
        private void SwitchToGameScene() => NetworkManager.singleton.ServerChangeScene("Game");
    }
}