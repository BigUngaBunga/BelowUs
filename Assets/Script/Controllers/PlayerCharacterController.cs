using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")]
        public FloatReference moveSpeed;
        public FloatReference jumpForce;
        public FloatReference climbSpeed;

        public InputActionAsset playerActions;

        public LayerMask ladderMask;
        public LayerMask groundMask;

        private bool isClimbing;
        private readonly float groundBuffer = 0.05f;

        private bool jumpRequest;
        private bool grounded;
        private Vector2 boxSize;

        private Rigidbody2D rb = null;
        private Vector2 playerSize;

        private PlayerAction action;
        private float horizontalInput;
        private float verticalInput;

        public StationController Station { get; set; }
        private bool IsInStation() => Station != null && Station.StationPlayerController == gameObject;

        private readonly bool debugMovement = false;

        private void Awake()
        {
            action = new PlayerAction();
            PlayerAction.PlayerActions playerAction = action.Player;

            playerAction.Move.performed += OnMove;
            playerAction.JumpClimbUp.performed += OnJump;
            playerAction.ClimbDown.performed += OnClimbDown;

            //In order to stop climbing when the button is released
            playerAction.JumpClimbUp.canceled += OnJump;
            playerAction.ClimbDown.canceled += OnClimbDown;

            playerAction.Enable();

            rb = GetComponent<Rigidbody2D>();
            playerSize = GetComponent<BoxCollider2D>().size;
            boxSize = new Vector2(playerSize.x, groundBuffer);
        }

        private void FixedUpdate()
        {
            if (!IsInStation())
            {
                HorizontalMovement();
                HandleClimbingBool();
                HandleJumping();
                HandleClimbing();
            }
        }

        private void OnEnable() => action?.Enable();

        private void OnDisable() => action?.Disable();

        #region Events
        public void OnMove(InputAction.CallbackContext value)
        {
            if (!PauseMenu.IsOpen)
                horizontalInput = value.ReadValue<float>();

            if (debugMovement)
                Debug.Log("OnMove Ran! And " + nameof(horizontalInput) + " is: " + horizontalInput);
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (!PauseMenu.IsOpen)
            {
                if (!isClimbing && grounded && value.ReadValue<float>() > 0)
                    jumpRequest = true;
                else if (isClimbing || verticalInput > 0)
                    verticalInput = value.ReadValue<float>();
            }
        }

        public void OnClimbDown(InputAction.CallbackContext value)
        {
            if (isClimbing && !PauseMenu.IsOpen)
                verticalInput = -value.ReadValue<float>();
        }
        #endregion

        private void HorizontalMovement()
        {
            float horizontalMovement = horizontalInput * moveSpeed.Value * Time.deltaTime;
            rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);

            if (!Mathf.Approximately(0, horizontalMovement))
                transform.rotation = horizontalMovement > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        private void HandleClimbingBool() => isClimbing = rb.IsTouchingLayers(ladderMask);

        private void HandleClimbing()
        {
            if (isClimbing)
            {
                rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed.Value * Time.deltaTime);

                if (rb.gravityScale != 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0); //Stops negative velocity
                }
            }
            else
            {
                rb.gravityScale = 1;
                verticalInput = 0;
            }
                
        }

        private void HandleJumping()
        {
            // Handles jumping
            if (jumpRequest)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
                grounded = false;
            }
            else
            {
                Vector2 boxCenter = (Vector2)transform.position + (Vector2.down * (playerSize.y + boxSize.y) * 0.5f);
                grounded = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundMask) != null;
            }
        }
    }
}
