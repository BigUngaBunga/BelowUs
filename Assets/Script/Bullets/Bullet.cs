using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float velocity = 20;

    private void Start()
    {
        rb.velocity = transform.right * velocity;
        rb.velocity = transform.up * velocity;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.name);
        Destroy(gameObject);
    }
}
