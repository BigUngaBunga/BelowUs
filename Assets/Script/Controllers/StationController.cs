using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : NetworkBehaviour
    {
        [SerializeField] protected CameraController cameraController;
        public CameraController Controller => cameraController;

        [SerializeField] protected Button leaveButton;
        [SerializeField] protected Button controlsButton;

        public Button LeaveButton => leaveButton;

        protected new Camera camera;
        private string playerTag;
        protected float playerCameraSize;
        private readonly float submarineCameraSize = 14;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] protected string switchTag;

        public string SwitchTag => switchTag;
       
        [SerializeField] [SyncVar] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        public bool IsOccupied => stationPlayerController != null;

        private void Start()
        {
            playerTag = ReferenceManager.Singleton.LocalPlayerTag;
            camera = cameraController.GetComponentInParent<Camera>();
            playerCameraSize = camera.orthographicSize;
        }

        public virtual void Enter(GameObject player)
        {
            if(!IsOccupied)
            {
                if (switchTag != null)
                    ChangeCameraToTag(switchTag);

                LeaveButton.gameObject.SetActive(true);
                leaveButton.onClick.AddListener(Leave);
                EnterStation(player);
            }
        }

        protected virtual void EnterStation(GameObject player)
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
        public void SetStationPlayerController(GameObject player) => stationPlayerController = player;
    }
}
