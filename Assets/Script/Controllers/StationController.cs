using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : NetworkBehaviour
    {
        [SerializeField] private CameraController cameraController;
        public CameraController Controller => cameraController;

        [SerializeField] private Button leaveButton;
        public Button LeaveButton => leaveButton;

        [SerializeField] [TagSelector] private string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] private string switchTag;
        public string PlayerTag => playerTag;
        public string SwitchTag => switchTag;
       
        [SerializeField] [SyncVar] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        [SerializeField] private bool isAGenerator;

        private bool IsOccupied => stationPlayerController != null;

        public void Enter(GameObject player)
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

                if (!isAGenerator)
                    cameraController.SwitchTarget(switchTag);
                else
                {
                    //TODO display generator UI
                }
            }
        }

        public void Leave()
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
