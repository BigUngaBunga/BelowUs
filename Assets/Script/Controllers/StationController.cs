using Mirror;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : NetworkBehaviour
    {
        [SerializeField] protected CameraController cameraController;
        [SerializeField] [MustBeAssigned] protected Button leaveButton;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [Tag] protected string switchTag;
        [SerializeField] private bool giveAuthority;

        [SerializeField] [ConditionalField(nameof(giveAuthority))] [MustBeAssigned] private GameObject controlObject;
        
        private NetworkIdentity controlObjNetworkIdentity;

        public CameraController Controller => cameraController;
        public Button LeaveButton => leaveButton;

        protected new Camera camera;
        private string playerTag;
        protected float playerCameraSize;
        private readonly float submarineCameraSize = 14;

        public string SwitchTag => switchTag;

        [SyncVar] private NetworkIdentity stationPlayerController = null;
        public NetworkIdentity StationPlayerController => stationPlayerController;

        public bool IsOccupied => stationPlayerController != null;

        private readonly bool debug = false;

        private void Start()
        {
            playerTag = ReferenceManager.Singleton.LocalPlayerTag;
            camera = cameraController.GetComponentInParent<Camera>();
            playerCameraSize = camera.orthographicSize;

            if (giveAuthority && controlObject != null)
            {
                controlObjNetworkIdentity = controlObject.GetComponent<NetworkIdentity>();
            }
        }

        public virtual void Enter(NetworkIdentity player)
        {
            if (!IsOccupied)
            {
                if (switchTag != null)
                    ChangeCameraToTag(switchTag);

                LeaveButton.gameObject.SetActive(true);
                LeaveButton.onClick.AddListener(Leave);

                SetButtonActiveStatus(true);
                ChangeCameraToTag(switchTag);
            }
        }

        public virtual void Leave()
        {
            stationPlayerController = null;

            SetButtonActiveStatus(false);
            ChangeCameraToTag(playerTag);
        }

        protected virtual void ChangeCameraToTag(string tag)
        {
            if (playerTag.Equals(tag))
            {
                cameraController.SwitchTarget(playerTag);
                camera.orthographicSize = playerCameraSize;
            }

            else
            {
                cameraController.SwitchTarget(switchTag);
                camera.orthographicSize = submarineCameraSize;
            }
        }

        protected virtual void SetButtonActiveStatus(bool activate) => LeaveButton.gameObject.SetActive(activate);

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
