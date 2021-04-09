using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SImplePlayerController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    Vector2 velocity;
    int speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = 7;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
