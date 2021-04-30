using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : MonoBehaviour
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
       
        [SerializeField] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        [SerializeField] private PlayerInput controllerPlayerInput = null;

        [SerializeField]  private bool isOccupied;
        [SerializeField] private bool isAGenerator;


        public void Enter(PlayerInput player)
        {
            if(!isOccupied && controllerPlayerInput == null)
            {
                controllerPlayerInput = player;
                stationPlayerController = player.gameObject;
                LeaveButton.gameObject.SetActive(true);
                controllerPlayerInput.enabled = false;
                isOccupied = true;
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
            controllerPlayerInput.enabled = true;
            controllerPlayerInput = null;
            stationPlayerController = null;
            cameraController.SwitchTarget(playerTag);
            LeaveButton.gameObject.SetActive(false);
            isOccupied = false;
            leaveButton.onClick.RemoveListener(Leave);
        }
    }
}


