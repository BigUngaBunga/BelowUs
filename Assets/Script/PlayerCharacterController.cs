using Mirror;
using UnityEngine;

public class PlayerCharacterController : NetworkBehaviour
{
    [SerializeField] private FloatReference MovementSpeed;
    [SerializeField] private FloatReference JumpForce;
    private Rigidbody2D CharacterRigidBody;

    private void Start()
    {
        CharacterRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Exit from update if this is not the local player
        if (!isLocalPlayer) return;

        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed.Value;

        // Turns character
        if (!Mathf.Approximately(0, movement))
            transform.rotation = movement > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        // Handles jumping
        if (Input.GetButtonDown("Jump") && Mathf.Abs(CharacterRigidBody.velocity.y) < 0.001)
            CharacterRigidBody.AddForce(new Vector2(0, JumpForce.Value), ForceMode2D.Impulse);
    }
}
