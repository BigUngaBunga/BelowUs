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
        private PlayerAction.PlayerActions playerAction;

        private NetworkIdentity identity;

        [SerializeField] private NetworkBehaviour currentStationController;
        [SerializeField] private CameraController cameraController;

        private bool debug = true;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            identity = GetComponent<NetworkIdentity>();
            cameraController = FindObjectOfType<CameraController>();
            leaveButton = GameObject.Find("UI").transform.Find("LeaveButton").GetComponent<Button>();
            leaveButton.onClick.AddListener(LeftController);

            PlayerAction action = new PlayerAction();
            playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();
        }

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
            {
                if (debug)
                    Debug.Log(nameof(OnStationClick) + " returned because " + nameof(currentStation) + " is null!");

                return;
            }

            StationController controller = currentStation.GetComponent<StationController>();

            if (controller != null)
                HandleStationController(controller);
            else
                HandleGeneratorController(currentStation.GetComponent<GeneratorController>());
        }

        private void HandleStationController(StationController controller)
        {
            if (debug && PauseMenu.IsOpen)
                Debug.Log(nameof(OnStationClick) + " returned because of " + nameof(PauseMenu.IsOpen) + " is open!");
            else if (debug && controller.IsOccupied)
                Debug.Log(nameof(OnStationClick) + " returned because " + nameof(controller.StationPlayerController) + " is not null");

            if (!PauseMenu.IsOpen && !controller.IsOccupied)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                if (identity == null)
                    Debug.LogError(nameof(identity) + " is null!");

                if (controller.debug)
                    Debug.Log(identity);

                currentStationController = controller;
                leaveButton.gameObject.SetActive(true);
                cameraController.SwitchTarget();

                playerAction.LeaveStation.performed += LeftStationControllerBtn;
                PauseMenu.IsEnabled = false;

                if (isServer)
                    controller.SetStationPlayerController(identity);
                else if (isClient)
                    controller.SetStationPlayerControllerCMD(identity);
            }
        }

        private void HandleGeneratorController(GeneratorController controller)
        {
            if (!PauseMenu.IsOpen && !controller.IsOccupied)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                if (identity == null)
                    Debug.LogError(nameof(identity) + " is null!");

                currentStationController = controller;
                leaveButton.gameObject.SetActive(true);

                playerAction.LeaveStation.performed += LeftGeneratorControllerBtn;
                PauseMenu.IsEnabled = false;

                controller.Enter(identity);
            }
        }

        private void LeftController()
        {
            if (currentStationController == null)
                return;

            if (currentStationController.GetType() == typeof(StationController) || currentStationController.GetType() == typeof(SubController))
                LeftStationController();
            else
                LeftGeneratorController();
        }

        private void LeftStationControllerBtn(InputAction.CallbackContext value) => LeftStationController();

        private void LeftStationController()
        {
            if (currentStationController == null)
                return;

            leaveButton.gameObject.SetActive(false);
            cameraController.SwitchTarget();

            playerAction.LeaveStation.performed -= LeftStationControllerBtn;
            PauseMenu.IsEnabled = true;

            StationController station = (StationController)currentStationController;

            if (!isServer)
                station.SetStationPlayerControllerCMD(null);
            else
                station.SetStationPlayerController(null);
        }

        private void LeftGeneratorControllerBtn(InputAction.CallbackContext value) => LeftGeneratorController();

        private void LeftGeneratorController()
        {
            if (currentStationController == null)
                return;

            leaveButton.gameObject.SetActive(false);

            playerAction.LeaveStation.performed -= LeftGeneratorControllerBtn;
            PauseMenu.IsEnabled = true;

            ((GeneratorController)currentStationController).Leave();
            currentStationController = null;
        }

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
    }
}
