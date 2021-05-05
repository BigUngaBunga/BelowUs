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
        public Button LeaveButton => leaveButton;

        [SerializeField] [TagSelector] protected string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] protected string switchTag;
        public string PlayerTag => playerTag;
        public string SwitchTag => switchTag;
       
        [SerializeField] [SyncVar] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        protected bool IsOccupied => stationPlayerController != null;

        public virtual void Enter(GameObject player)
        {
            if(!IsOccupied)
            {
                if (!isServer)
                {
                    SetStationPlayerController();
                }
                else
                    stationPlayerController = player;
                LeaveButton.gameObject.SetActive(true);
                leaveButton.onClick.AddListener(Leave);
            }
        }

        public virtual void Leave()
        {
            stationPlayerController = null;
            cameraController.SwitchTarget(playerTag);
            LeaveButton.gameObject.SetActive(false);
            leaveButton.onClick.RemoveListener(Leave);
        }

        [Command]
        private void SetStationPlayerController()
        {

        }
    }
}
