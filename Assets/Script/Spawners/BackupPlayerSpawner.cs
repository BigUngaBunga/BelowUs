using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class BackupPlayerSpawner : MonoBehaviour
    {
        [TagSelector] [SerializeField] private string playerTag;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject networkManager;
        public GameObject PlayerPrefab => playerPrefab;
        public GameObject NetManager => networkManager;

        private NetworkManager manager;

        // Start is called before the first frame update
        private void Start()
        {
            manager = NetworkManager.singleton;
            Invoke(nameof(SpawnPlayer), 0.2f);
        }

        private void SpawnPlayer()
        {
            if (manager == null && GameObject.FindGameObjectsWithTag(playerTag).Length == 0)
            {
                manager = Instantiate(networkManager).GetComponent<NetworkManager>();
                manager.StartHost();
            }

            Destroy(this);
        }
    }
}

