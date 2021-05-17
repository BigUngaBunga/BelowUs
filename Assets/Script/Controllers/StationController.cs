using Mirror;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : NetworkBehaviour
    {
        [SerializeField] protected CameraController cameraController;

        [Tooltip("The tag of the object that the camera should switch to on enter. For example station or weapon.")]
        [SerializeField] [Tag] protected string switchTag;

        [SerializeField] private bool giveAuthority;

        [SerializeField] [ConditionalField(nameof(giveAuthority))] [MustBeAssigned] private GameObject controlObject;
        
        [SerializeField] private NetworkIdentity controlObjNetworkIdentity;

        public CameraController Controller => cameraController;

        protected new Camera camera;

        [SerializeField] [SyncVar] private NetworkIdentity stationPlayerController = null;
        public NetworkIdentity StationPlayerController => stationPlayerController;

        [SerializeField] public bool IsOccupied => stationPlayerController != null;
        
        public bool debug = false;

        private void Start()
        {
            if (giveAuthority && controlObject != null)
                controlObjNetworkIdentity = controlObject.GetComponent<NetworkIdentity>();
        }

        [Command(requiresAuthority = false)] public void SetStationPlayerControllerCMD(NetworkIdentity player) => SetStationPlayerController(player);

        [Server]
        public void SetStationPlayerController(NetworkIdentity player)
        {
            stationPlayerController = player;

            if (giveAuthority)
            {
                if (debug && player != null)
                    Debug.Log(player.gameObject.name + " " + player.netId);

                if (player == isServer && player.hasAuthority)
                {
                    if (debug)
                        Debug.Log("Player is server and already has authority therefore nothing happens!");
                }

                else if (player == null)
                {
                    if (debug)
                        Debug.Log("Removed client authority!");

                    controlObjNetworkIdentity.RemoveClientAuthority();
                }

                else
                {
                    if (debug)
                        Debug.Log("Assigned client authority to player: " + player.gameObject.name + "!");

                    controlObjNetworkIdentity.AssignClientAuthority(StationPlayerController.connectionToClient);
                }
            }                
        }
    }
}
