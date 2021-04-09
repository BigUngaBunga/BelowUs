using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SImplePlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 velocity;
    int speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = 7;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
