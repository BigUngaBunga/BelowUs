using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float velocity = 20;
        [SerializeField] private float damage;
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
            if (collider.gameObject.tag == "enemy" || collider.gameObject.tag == "submarine")
                Destroy(this);
        }
    }
}

