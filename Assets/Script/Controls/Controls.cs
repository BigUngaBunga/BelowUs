using Mirror;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class Controls : NetworkBehaviour
    {
        [ReadOnly] [SerializeField] private Transform currentStation;
        private Rigidbody2D rb;

        [SyncVar] private NetworkIdentity identity;
        private StationController currentStationController;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

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

                currentStationController = controller;
                currentStationController.Enter(identity);

                if (isServer)
                {
                    currentStationController.SetStationPlayerController(identity);
                    currentStationController.LeaveButton.onClick.AddListener(SetPControllerToNull);
                }
                else if (isClient)
                {
                    SuccessfullyEnteredStation(identity);
                    currentStationController.LeaveButton.onClick.AddListener(SuccessfullyLeftStation);
                }
            }
        }

        public void SetIdentity(NetworkIdentity identity) => this.identity = identity;

        [Command] private void SuccessfullyEnteredStation(NetworkIdentity inputIdentity) => currentStationController.SetStationPlayerController(inputIdentity);

        [Command]
        private void SuccessfullyLeftStation()
        {
            SetPControllerToNull();
            currentStationController.LeaveButton.onClick.RemoveListener(SuccessfullyLeftStation);
        }

        [Server] private void SetPControllerToNull() => currentStationController.SetStationPlayerController(null);

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
