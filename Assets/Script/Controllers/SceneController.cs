using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using RoboRyanTron.Unite2017.Events;

namespace BelowUs
{
    public class SceneController : NetworkBehaviour
    {
        [SerializeField] private ShipResource submarineHealth;
        [SerializeField] private ShipResource submarineOxygen;
        [SerializeField] private FloatVariable gold;

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
        public void SwitchToVillageScene()
        {
            SaveSessionGold();
            NetworkManager.singleton.ServerChangeScene("Village");
        }

        [Command(requiresAuthority = false)]
        public void CommandSwitchToGameScene() => SwitchToGameScene();

        [Server]
        private void SwitchToGameScene() => NetworkManager.singleton.ServerChangeScene("Game");

        private void SaveSessionGold() => gold.ApplyChange(GameObject.Find("CurrentGold").GetComponent<ShipResource>().CurrentValue);
    }
}