using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D cd;

        [SerializeField] private float velocity = 20;
        [SerializeField] private float damage;
        [SerializeField] private int expirationTime;
        [SerializeField] private bool invertDirection;

        public float Damage => damage;

        [Server]
        private void Start()
        {
            int invertDirectionInt = invertDirection ? -1 : 1;
            Vector2 xVelocity = transform.right * velocity * invertDirectionInt;
            Vector2 yVelocity = transform.up * velocity * invertDirectionInt;

            //TODO doesn't yvelocity instantly overwrite xvelocity?
            rb.velocity = xVelocity;
            rb.velocity = yVelocity;

            Invoke(nameof(EnableCollision), 0.25f); //This prevents the bullet from colliding with it's creator.
            Invoke(nameof(Expire), expirationTime);
        }

        /**
         * Activates bullet collision.
         */
        private void EnableCollision() => cd.enabled = true;

        private void Expire() => Destroy(gameObject);
    }
}

