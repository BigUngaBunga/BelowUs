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

        [SerializeField] private float collisionDelay = 0.05f;

        public float Damage => damage;

        private readonly bool debug = false;

        [Server]
        private void Start()
        {
            int invertDirectionInt = invertDirection ? -1 : 1;
            
            Vector2 yVelocity = transform.up * velocity * invertDirectionInt;

            if (debug)
                Debug.Log("Initial Velocity " + rb.velocity);

            //TODO check and remove this and maybe the debug bool too since xVelocity is instantly overwritten by yVelocity
            //Vector2 xVelocity = transform.right * velocity * invertDirectionInt;
            //rb.velocity = xVelocity;
            //if (debug)
            //Debug.Log("Velocity after " + nameof(xVelocity) + " " + rb.velocity);

            rb.velocity = yVelocity;

            if (debug)
                Debug.Log("Velocity after " + nameof(yVelocity) + " " + rb.velocity);

            Invoke(nameof(EnableCollision), collisionDelay); //This prevents the bullet from colliding with it's creator.
            Invoke(nameof(Expire), expirationTime);
        }

        /**
         * Activates bullet collision.
         */
        private void EnableCollision() => cd.enabled = true;

        private void Expire() => Destroy(gameObject);
    }
}

