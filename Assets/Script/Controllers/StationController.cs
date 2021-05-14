using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : NetworkBehaviour
    {
        [SerializeField] protected CameraController cameraController;
        [SerializeField] protected Button leaveButton;
        [SerializeField] protected Button controlsButton;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] protected string switchTag;
        [SerializeField] private bool giveAuthority;

        [SerializeField] private GameObject controlObject;
        private NetworkIdentity controlObjNetworkIdentity;
        [SerializeField] private GameObject controllingPlayer; //Unnecessary but useful for debugging

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

        private bool debug = true;

        private void Start()
        {
            playerTag = ReferenceManager.Singleton.LocalPlayerTag;
            camera = cameraController.GetComponentInParent<Camera>();
            playerCameraSize = camera.orthographicSize;

            if (controlObject != null)
                controlObjNetworkIdentity = controlObject.GetComponent<NetworkIdentity>();

            if (debug)
                NetworkIdentity.clientAuthorityCallback += UpdateControllingPlayer;
        }

        /**
         * Unnecessary but useful for debugging.
         */
        private void UpdateControllingPlayer(NetworkConnection conn, NetworkIdentity identity, bool authorityState)
        {
            
        }

        public virtual void Enter(NetworkIdentity player)
        {
            if (!IsOccupied)
            {
                if (switchTag != null)
                    ChangeCameraToTag(switchTag);

                LeaveButton.gameObject.SetActive(true);
                leaveButton.onClick.AddListener(Leave);
                EnterStation(player);
            }
        }

        protected virtual void EnterStation(NetworkIdentity player)
        {
            if (isServer)
                SetStationPlayerController(player);
            SetButtonActiveStatus(true);
            ChangeCameraToTag(switchTag);
        }

        public virtual void Leave()
        {
            stationPlayerController = null;
            ChangeCameraToTag(playerTag);
            SetButtonActiveStatus(false);
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

        private void SetButtonActiveStatus(bool activate)
        {
            LeaveButton.gameObject.SetActive(activate);

            if (controlsButton != null)
                controlsButton.gameObject.SetActive(activate);

            if (activate)
                leaveButton.onClick.AddListener(Leave);
            else
                leaveButton.onClick.RemoveListener(Leave);
        }

        [Server]
        public void SetStationPlayerController(NetworkIdentity player)
        {
            stationPlayerController = player;

            if (player == null && !hasAuthority)
                controlObjNetworkIdentity.RemoveClientAuthority();
            if (player != isServer && giveAuthority)
                controlObjNetworkIdentity.AssignClientAuthority(StationPlayerController.connectionToClient);
        }
    }
}
