using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class Controls : NetworkBehaviour
    {
        [SerializeField] private Transform currentStation;
        private Rigidbody2D rb;
        private NetworkConnection networkConnection;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            if (isServer)
                networkConnection = GetComponent<NetworkIdentity>().connectionToClient;
            else
                networkConnection = GetComponent<NetworkIdentity>().connectionToServer;

            PlayerAction action = new PlayerAction();
            PlayerAction.PlayerActions playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();
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

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
                return;

            StationController controller = currentStation.GetComponent<StationController>();

            if (!PauseMenu.IsOpen && controller != null && controller.StationPlayerController == null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                controller.Enter(networkConnection.identity);

                if (isClient && hasAuthority)
                    SuccessfullyEnteredStation(controller);

                if (hasAuthority)
                    controller.LeaveButton.onClick.AddListener(SuccessfullyLeftStation);
            }
        }

        [Command]
        private void SuccessfullyEnteredStation(StationController controller) => controller.SetStationPlayerController(networkConnection.identity);

        [Command]
        private void SuccessfullyLeftStation()
        {
            StationController controller = currentStation.GetComponent<StationController>();
            controller.LeaveButton.onClick.RemoveListener(SuccessfullyLeftStation);
            controller.SetStationPlayerController(null);
        }
    }
}
