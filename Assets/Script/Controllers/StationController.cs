using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;

namespace BelowUs
{
    public class StationController : MonoBehaviour
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
       
        [SerializeField] protected GameObject stationPlayerController = null;
        public GameObject StationPlayerController => stationPlayerController;

        [SerializeField] protected PlayerInput controllerPlayerInput = null;
        [SerializeField] protected bool isOccupied;


        public void Enter(PlayerInput player)
        {
            if (!isOccupied && controllerPlayerInput == null)
                EnterStation(player);
        }

        protected virtual void EnterStation(PlayerInput player)
        {
            controllerPlayerInput = player;
            stationPlayerController = player.gameObject;
            LeaveButton.gameObject.SetActive(true);
            controllerPlayerInput.enabled = false;
            isOccupied = true;
            leaveButton.onClick.AddListener(Leave);
            cameraController.SwitchTarget(switchTag);
        }

        public virtual void Leave()
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


