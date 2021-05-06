using Mirror;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BelowUs
{
    public class BackupPlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject networkManager;
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
            var refManager = ReferenceManager.Singleton;
            string playerTag = refManager.PlayerTag;
            string localPlayerTag = refManager.LocalPlayerTag;

            if (manager == null && GameObject.FindGameObjectWithTag(playerTag) == null && GameObject.FindGameObjectWithTag(localPlayerTag) == null)
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

