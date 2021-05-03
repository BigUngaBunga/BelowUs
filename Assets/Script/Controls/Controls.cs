using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class Controls : MonoBehaviour
    {
        [SerializeField] private Transform currentStation;
        [SerializeField] private LayerMask stationMask;
        private Rigidbody2D rb;
        private PlayerAction action;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            action = new PlayerAction();
            PlayerAction.PlayerActions playerAction = action.Player;

            playerAction.EnterStation.performed += OnStationClick;

            playerAction.Enable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!rb.IsTouchingLayers(stationMask))
                return;

            currentStation = collision.transform.parent;
        }

        private void OnTriggerExit2D(Collider2D collision) => currentStation = null;

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (currentStation == null)
                return;

            StationController cont = currentStation.GetComponent<StationController>();

            if (!PauseMenu.IsOpen && cont != null)
                cont.Enter(gameObject);
        }
    }
}
