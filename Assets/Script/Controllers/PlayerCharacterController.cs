using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace BelowUs
{
    public class PlayerCharacterController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private FloatReference movementSpeed;
        [SerializeField] private FloatReference jumpForce;
        [SerializeField] private FloatReference climbingSpeed;

        public FloatReference MovementSpeed => movementSpeed;
        public FloatReference JumpForce => jumpForce;
        public FloatReference ClimbingSpeed => climbingSpeed;

        [SerializeField] private LayerMask ladderMask;

        [SerializeField] private bool isClimbing;

        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundBuffer = 0.05f;

        private Rigidbody2D rb;
        private bool jumpRequest;
        private bool grounded;
        private Vector2 playerSize;
        private Vector2 boxSize;

        [SerializeField] private float horizontalInput;
        private float verticalInput;

        private PlayerInput input;

        [SerializeField] private LayerMask stationMask;
        [SerializeField] public StationController Station { private get; set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerSize = GetComponent<BoxCollider2D>().size;
            boxSize = new Vector2(playerSize.x, groundBuffer);
            input = GetComponent<PlayerInput>();

            if (input.uiInputModule == null)
                input.uiInputModule = FindObjectOfType<InputSystemUIInputModule>();
        }

        private void FixedUpdate()
        {
            // Exit from update if this is not the local player
            if (!isLocalPlayer) return;

            HorizontalMovement();
            HandleClimbingBool();
            HandleJumping();
            HandleClimbing();
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
        }

        #region Events
        public void OnMove(InputAction.CallbackContext value)
        {
            if (!PauseMenu.IsOpen)
                horizontalInput = value.ReadValue<Vector2>().x;
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (!PauseMenu.IsOpen)
            {
                if (isClimbing)
                    verticalInput = value.ReadValue<float>();
                else if (grounded)
                    jumpRequest = true;
            }
        }

        public void OnClimbDown(InputAction.CallbackContext value)
        {
            if (isClimbing && !PauseMenu.IsOpen)
                verticalInput = -value.ReadValue<float>();
        }

        public void OnStationClick(InputAction.CallbackContext value)
        {
            if (!rb.IsTouchingLayers(stationMask))
                return;            

            if (!PauseMenu.IsOpen && Station != null)
                Station.Enter(input);
        }
        #endregion

        private void HorizontalMovement()
        {
            float horizontalMovement = horizontalInput * movementSpeed.Value * Time.deltaTime;
            rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);

            if (!Mathf.Approximately(0, horizontalMovement))
                transform.rotation = horizontalMovement > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        private void HandleClimbingBool() => isClimbing = rb.IsTouchingLayers(ladderMask);

        private void HandleClimbing()
        {
            if (isClimbing)
            {
                rb.velocity = new Vector2(rb.velocity.x, verticalInput);

                if (rb.gravityScale != 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(rb.velocity.x, 0); //Stops negative velocity
                }
            }
            else
                rb.gravityScale = 1;
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
                Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
                grounded = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundMask) != null;
            }
        }
    }

}

