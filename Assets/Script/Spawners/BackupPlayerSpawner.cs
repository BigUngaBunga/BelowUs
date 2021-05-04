using Mirror;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BelowUs
{
    public class BackupPlayerSpawner : MonoBehaviour
    {
        [TagSelector] [SerializeField] private string localPlayerTag;
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
            if (manager == null && GameObject.FindGameObjectWithTag(localPlayerTag) == null)
            {
                try
                {
                    manager = Instantiate(networkManager).GetComponent<NetworkManager>();
                    manager.StartHost();
                }
                catch (SocketException)
                {
                    Destroy(manager.gameObject);
                    SceneManager.LoadScene(0);
                }
            }

            Destroy(this);
        }
    }
}
