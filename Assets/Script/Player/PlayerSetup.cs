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

        public FloatReference MoveSpeed => moveSpeed;
        public FloatReference JumpForce => jumpForce;
        public FloatReference ClimbSpeed => climbSpeed;

        public void Start()
        {
            if (!isLocalPlayer)
                Destroy(this);
        }

        public override void OnStartAuthority()
        {
            tag = "LocalPlayer";

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
            controller.SetMovementSpeed(moveSpeed);
            controller.SetJumpForce(jumpForce);
            controller.SetClimbSpeed(climbSpeed);
        }
    }
}