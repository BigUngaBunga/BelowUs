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

        private string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] protected string switchTag;

        public string SwitchTag => switchTag;
       
        [SerializeField] [SyncVar] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        public bool IsOccupied => stationPlayerController != null;

        private void Start() => playerTag = ReferenceManager.Singleton.PlayerTag;

        public virtual void Enter(GameObject player)
        {
            if(!IsOccupied)
            {
                if (!isServer)
                    SetStationPlayerController();
                else
                    stationPlayerController = player;

                if (switchTag != null)
                    cameraController.SwitchTarget(switchTag);

                LeaveButton.gameObject.SetActive(true);
                leaveButton.onClick.AddListener(Leave);
                EnterStation(player);
            }
        }

        protected virtual void EnterStation(GameObject player)//PlayerInput player)
        {
            stationPlayerController = player.gameObject;
            SetButtonActiveStatus(true);
            cameraController.SwitchTarget(SwitchTag);
            Debug.Log("Current tag: " + SwitchTag.ToString());
        }

        public virtual void Leave()
        {
            stationPlayerController = null;

            //TODO byt till vanliga SwitchTarget, detta är enbart för att få speltestet att fungera
            if (playerTag != null)
                cameraController.SwitchTarget(true);
            //cameraController.SwitchTarget(playerTag);

            SetButtonActiveStatus(false);
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

        [Command]
        private void SetStationPlayerController()
        {

        }
    }
}
