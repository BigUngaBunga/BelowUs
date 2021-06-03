using Mirror;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BelowUs
{
    public class Controls : NetworkBehaviour
    {
        [ReadOnly] [SerializeField] private Transform currentStation;
        private Rigidbody2D rb;
        private Button leaveButton;
        private GameObject controlsButton;
        private PlayerAction.PlayerActions playerAction;

        private NetworkIdentity identity;

        [SerializeField] private NetworkBehaviour currentStationController;
        [SerializeField] private CameraController cameraController;


        private PlayerCharacterController playerController;
        private readonly bool debug = true;

        public bool IsInStation => currentStationController != null;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            identity = GetComponent<NetworkIdentity>();
            cameraController = FindObjectOfType<CameraController>();
            playerController = GetComponent<PlayerCharacterController>();
            leaveButton = GameObject.Find("UI").transform.Find("LeaveButton").GetComponent<Button>();
            leaveButton.onClick.AddListener(LeftController);

            controlsButton = GameObject.Find("Game/UI/SubmarineControls");

            PlayerAction action = new PlayerAction();
            playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();

            PauseMenu.IsEnabled = true;
        }

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
            {
                if (debug)
                    Debug.Log(nameof(OnStationClick) + " returned because " + nameof(currentStation) + " is null!");

                return;
            }

            HandleBaseStationController(currentStation.GetComponent<BaseStationController>());
        }

        private void HandleBaseStationController(BaseStationController controller)
        {
            if (!PauseMenu.IsOpen && !controller.IsOccupied)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                if (identity == null)
                    Debug.LogError(nameof(identity) + " is null!");

                currentStationController = controller;
                leaveButton.gameObject.SetActive(true);

                playerAction.LeaveStation.performed += LeftStationControllerBtn;
                PauseMenu.IsEnabled = false;

                if (controller is StationController stationController)
                {
                    cameraController.SwitchTarget();
                    HadleControlsButton(true);
                    if (isServer)
                        stationController.SetStationPlayerController(identity);
                    else if (isClient)
                        stationController.SetStationPlayerControllerCMD(identity);

                    if (stationController is CannonController cannonController)
                        cannonController.ActivateUI(true);
                }
                else if (controller.GetType() == typeof(GeneratorController))
                    ((GeneratorController)controller).Enter(identity);
                else if (controller.GetType() == typeof(VillageController))
                    ((VillageController)controller).Enter(identity);

                playerController.IsInStation = true;
            }
        }

        private void LeftController()
        {
            if (currentStationController == null)
                return;

            leaveButton.gameObject.SetActive(false);

            if (currentStationController is StationController)
                HadleControlsButton(false);


            playerAction.LeaveStation.performed -= LeftStationControllerBtn;
            PauseMenu.IsEnabled = true;

            if (currentStationController is StationController station)
            {
                if (!isServer)
                    station.SetStationPlayerControllerCMD(null);
                else
                    station.SetStationPlayerController(null);

                if (station is CannonController cannonController)
                    cannonController.ActivateUI(false);

                cameraController.SwitchTarget();
            }


            if (currentStationController is GeneratorController controller)
            {
                leaveButton.gameObject.SetActive(false);
                PauseMenu.IsEnabled = true;
                controller.Leave();
                currentStationController = null;
            }

            playerController.IsInStation = false;
        }

        private void LeftStationControllerBtn(InputAction.CallbackContext value) => LeftController();

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (CollisionIsAStation(collider))
                currentStation = collider.transform.parent;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (CollisionIsAStation(collider))
                currentStation = null;
        }

        private bool CollisionIsAStation(Collider2D collider)
        {
            string colliderTag = collider.transform.parent.gameObject.tag;
            return (colliderTag != null || colliderTag != ReferenceManager.Singleton.Untagged) && colliderTag == ReferenceManager.Singleton.StationTag;
        }

        private void HadleControlsButton(bool setActive)
        {
            if (!setActive && controlsButton.GetComponent<DisplayControlsButton>().ControlViewIsActive)
                controlsButton.GetComponent<Button>().onClick.Invoke();
            controlsButton.SetActive(setActive);   
        }
    }
}
