using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float velocity = 20;
    [SerializeField] float damage;
    public float Damage
    {
        get { return damage; }
    }

    [Server]
    private void Start()
    {
        rb.velocity = transform.right * velocity;
        rb.velocity = transform.up * velocity;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        Destroy(gameObject);

    }
}
