using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;
        public CameraController Controller { get { return cameraController; } }

        [SerializeField] private Button leaveButton;
        public Button LeaveButton { get { return leaveButton; } }


        [SerializeField] [TagSelector] private string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] private string switchTag;
        public string PlayerTag { get { return playerTag; } }
        public string SwitchTag { get { return switchTag; } }

       
        [SerializeField] private GameObject stationPlayerController = null;
        public GameObject StationPlayerController { get { return stationPlayerController; } }

        [SerializeField] private PlayerInput controllerPlayerInput = null;

        public void Enter(PlayerInput player)
        {
            if(controllerPlayerInput == null)
            {
                controllerPlayerInput = player;

                stationPlayerController = player.gameObject;
                cameraController.SwitchTarget(switchTag);
                LeaveButton.gameObject.SetActive(true);

                controllerPlayerInput.enabled = false;

                leaveButton.onClick.AddListener(Leave);
            }
        }

        public void Leave()
        {
            controllerPlayerInput.enabled = true;
            controllerPlayerInput = null;
            stationPlayerController = null;
            cameraController.SwitchTarget(playerTag);

            leaveButton.onClick.RemoveListener(Leave);
        }
    }
}


