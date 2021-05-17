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

        private NetworkIdentity identity;

        [SerializeField] private StationController currentStationController;
        [SerializeField] private CameraController cameraController;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            identity = GetComponent<NetworkIdentity>();
            cameraController = FindObjectOfType<CameraController>();
            leaveButton = GameObject.Find("UI").transform.Find("LeaveButton").GetComponent<Button>();
            leaveButton.onClick.AddListener(LeftStation);
                
            PlayerAction action = new PlayerAction();
            PlayerAction.PlayerActions playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();
        }

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
                return;

            StationController controller = currentStation.GetComponent<StationController>();

            if (!PauseMenu.IsOpen && controller != null && controller.StationPlayerController == null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

                if (identity == null)
                    Debug.LogError(nameof(identity) + " is null!");

                if (controller.debug)
                    Debug.Log(identity);

                currentStationController = controller;
                leaveButton.gameObject.SetActive(true);
                cameraController.SwitchTarget();

                if (isServer)
                    currentStationController.SetStationPlayerController(identity);
                else if (isClient)
                    currentStationController.SetStationPlayerControllerCMD(identity);
            }
        }

        private void LeftStation()
        {
            if (currentStationController == null)
                return;

            leaveButton.gameObject.SetActive(false);
            cameraController.SwitchTarget();

            if (!isServer)
                currentStationController.SetStationPlayerControllerCMD(null);
            else
                currentStationController.SetStationPlayerController(null);

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
