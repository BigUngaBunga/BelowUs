using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace BelowUs
{
    public class PlayerSetup : NetworkBehaviour
    {
        [Header("Character Controller")]
        [SerializeField] private FloatReference moveSpeed;
        [SerializeField] private FloatReference jumpForce;
        [SerializeField] private FloatReference climbSpeed;

        [SerializeField] private InputActionAsset playerActions;

        [SerializeField] private LayerMask ladderMask;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask stationMask;

        public void Start()
        {
            if (!isLocalPlayer)
                Destroy(this);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            AddPlayerInput();
            AddPlayerCharacterController();

            Destroy(this);
        }

        private void AddPlayerInput()
        {
            PlayerInput input = gameObject.AddComponent<PlayerInput>();
            input.actions = playerActions;
            input.currentActionMap = playerActions.actionMaps[0];
            input.defaultControlScheme = "Keyboard&Mouse";
            input.defaultActionMap = "Player";
            input.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
            input.enabled = true;

            input.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
        }

        private void AddPlayerCharacterController()
        {
            PlayerCharacterController controller = gameObject.AddComponent<PlayerCharacterController>();
            controller.moveSpeed = moveSpeed;
            controller.jumpForce = jumpForce;
            controller.climbSpeed = climbSpeed;

            controller.playerActions = playerActions;

            controller.ladderMask = ladderMask;
            controller.groundMask = groundMask;
            controller.stationMask = stationMask;
        }
    }
}