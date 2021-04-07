using Mirror;
using UnityEngine;

public class PlayerCharacterController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private FloatReference MovementSpeed;
    [SerializeField] private FloatReference JumpForce;
    [SerializeField] private FloatReference ClimbingSpeed;
    [SerializeField] private LayerMask ladderMask;

    [SerializeField] private bool isClimbing;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundBuffer = 0.05f;

    private Rigidbody2D rb;
    private bool jumpRequest;
    private bool grounded;
    private Vector2 playerSize;
    private Vector2 boxSize;

    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSize = GetComponent<BoxCollider2D>().size;
        boxSize = new Vector2(playerSize.x, groundBuffer);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && !isClimbing && grounded)
            jumpRequest = true;

        horizontalInput = Input.GetAxis("Horizontal");
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

    private void HorizontalMovement()
    {
        float horizontalMovement = horizontalInput * MovementSpeed.Value * Time.deltaTime;
        rb.velocity = new Vector2(horizontalMovement, rb.velocity.y);
        //rb.MovePosition((Vector2)transform.position + new Vector2(horizontalMovement, rb.velocity.y * Time.deltaTime));

        if (!Mathf.Approximately(0, horizontalMovement))
            transform.rotation = horizontalMovement > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void HandleClimbingBool()
    {
        if (rb.IsTouchingLayers(ladderMask))
            isClimbing = true;
        else
            isClimbing = false;
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            var verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime * ClimbingSpeed.Value;
            rb.velocity = new Vector2(rb.velocity.x, verticalMovement);

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
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
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
