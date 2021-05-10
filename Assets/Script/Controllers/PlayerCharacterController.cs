using UnityEngine;
using UnityEngine.InputSystem;

namespace BelowUs
{
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FloatReference moveSpeed;
        [SerializeField] private FloatReference jumpForce;
        [SerializeField] private FloatReference climbSpeed;

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
        private readonly bool debugJump = false;

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
            if (!IsInStation()) //TODO event that changes isInStation and not running the if statement in that case
            {
                HorizontalMovement();
                HandleClimbingBool();
                HandleJumping();
                HandleClimbing();
            }
        }

        private void OnEnable() => action?.Enable();

        private void OnDisable() => action?.Disable();

        #region Setters
        public void SetMovementSpeed(FloatReference ms) => moveSpeed = ms;
        public void SetJumpForce(FloatReference jf) => jumpForce = jf;
        public void SetClimbSpeed(FloatReference cs) => climbSpeed = cs;
        #endregion

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
            if (!PauseMenu.IsOpen && !IsInStation())
            {
                if (debugJump)
                    Debug.Log(nameof(isClimbing) + " is: " + isClimbing + "\n" + nameof(grounded) + " is " + grounded);

                if (!isClimbing && grounded && value.ReadValue<float>() > 0)
                    jumpRequest = true;
                else if (isClimbing || verticalInput > 0)
                    verticalInput = value.ReadValue<float>();

                if (debugJump)
                    Debug.Log("OnJump Ran! And " + nameof(jumpRequest) + " is " + jumpRequest);
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

        private void HandleClimbingBool() => isClimbing = rb.IsTouchingLayers(ReferenceManager.Singleton.LadderMask);

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
                grounded = Physics2D.OverlapBox(boxCenter, boxSize, 0f, ReferenceManager.Singleton.GroundMask) != null;
            }
        }
    }
}
