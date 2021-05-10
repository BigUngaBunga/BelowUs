using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class Controls : NetworkBehaviour
    {
        [SerializeField] private Transform currentStation;
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            PlayerAction action = new PlayerAction();
            PlayerAction.PlayerActions playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!rb.IsTouchingLayers(ReferenceManager.Singleton.StationMask))
                return;

            currentStation = collision.transform.parent;
        }

        private void OnTriggerExit2D() => currentStation = null;

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
                return;

            StationController cont = currentStation.GetComponent<StationController>();

            if (!PauseMenu.IsOpen && cont != null && cont.StationPlayerController == null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                cont.Enter(gameObject);

                if (isClient)
                    SuccessfullyEnteredStation(cont);

                cont.LeaveButton.onClick.AddListener(SuccessfullyLeftStation);
            }
        }

        [Command]
        private void SuccessfullyEnteredStation(StationController cont) => cont.SetStationPlayerController(gameObject);

        [Command]
        private void SuccessfullyLeftStation()
        {
            StationController cont = currentStation.GetComponent<StationController>();
            cont.LeaveButton.onClick.RemoveListener(SuccessfullyLeftStation);
            cont.SetStationPlayerController(null);
        }
    }
}
