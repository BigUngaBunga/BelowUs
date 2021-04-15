using UnityEngine;

public class SImplePlayerController : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    Vector2 velocity;
    int speed;
    void Start()
    {
        speed = 7;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void FixedUpdate()
    {
        rigidbody2d.MovePosition(rigidbody2d.position + velocity * Time.fixedDeltaTime);
    }
}
