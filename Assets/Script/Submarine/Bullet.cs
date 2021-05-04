using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 dir = new Vector2(transform.forward.x, transform.forward.y).normalized;
        speed = 5;
        rb.velocity = dir * speed;
    }
}
