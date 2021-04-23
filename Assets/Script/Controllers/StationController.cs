using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BelowUs
{
    public class StationController : MonoBehaviour
    {
        [SerializeField] private CameraController controller;
        public CameraController Controller { get { return controller; } }

        [SerializeField] private Button leaveButton;
        public Button LeaveButton { get { return leaveButton; } }


        [SerializeField] [TagSelector] private string playerTag;
        [Tooltip("The tag of the object that the camera should switch to on collision. For example station or weapon.")]
        [SerializeField] [TagSelector] private string switchTag;
        public string PlayerTag { get { return playerTag; } }
        public string SwitchTag { get { return switchTag; } }

        private PlayerInput stationController = null;

        public void Start()
        {
            leaveButton.onClick.AddListener(Leave);
        }

        public void Enter(PlayerInput player)
        {
            if(stationController == null)
            {
                stationController = player;
                stationController.enabled = false;
                controller.SwitchTarget(switchTag);
            }
        }

        public void Leave()
        {
            stationController.enabled = true;
            stationController = null;
            controller.SwitchTarget(playerTag);
        }
    }
}


