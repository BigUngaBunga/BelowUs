using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D cd;

        [SerializeField] private float velocity = 20;
        [SerializeField] private FloatVariable cannonDamage;
        [SerializeField] private float damage;
        [SerializeField] private int expirationTime;
        [SerializeField] private bool invertDirection;

        [SerializeField] private float collisionDelay = 0.05f;
        [SerializeField] private bool npc;

        public float Damage => cannonDamage == null ? damage : cannonDamage.Value;

        private readonly bool debug = false;

        private void Start()
        {
            if (isServer)
            {
                int invertDirectionInt = invertDirection ? -1 : 1;

                Vector2 yVelocity = transform.up * velocity * invertDirectionInt;

                if (debug)
                    Debug.Log("Initial Velocity " + rb.velocity);

                rb.velocity = yVelocity;

                if (debug)
                    Debug.Log("Velocity after " + nameof(yVelocity) + " " + rb.velocity);

                npc = CompareTag("EnemyBullet");

                Invoke(nameof(EnableCollision), collisionDelay); //This prevents the bullet from colliding with it's creator.
                Invoke(nameof(Expire), expirationTime);
            }
        }

        /**
         * Activates bullet collision.
         */
        private void EnableCollision() => cd.enabled = true;

        [Server] private void Expire() => NetworkServer.Destroy(gameObject);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(ReferenceManager.Singleton.SubmarineTag) || collision.CompareTag(ReferenceManager.Singleton.EnemyTag))
            {
                bool alliedShip = (npc && collision.CompareTag(ReferenceManager.Singleton.EnemyTag)) || (!npc && collision.CompareTag(ReferenceManager.Singleton.SubmarineTag));

                if (debug)
                {
                    string info = transform.name + " was hit by a ";
                    if (alliedShip)
                        Debug.Log(info + nameof(alliedShip));
                    else
                        Debug.Log(info + "enemy bullet");
                }

                if (!alliedShip)
                {
                    //Todo change this if collider position is standardized
                    ShipResource hullHealth = npc ? collision.gameObject.GetComponentInParent<ShipResource>() : collision.gameObject.GetComponent<ShipResource>();

                    if (debug)
                        Debug.Log("Damage is: " + damage + "\nHealth before: " + hullHealth.CurrentValue);

                    hullHealth.ApplyChange(-Damage);

                    if (debug)
                        Debug.Log("Health after: " + hullHealth.CurrentValue);

                    if (npc && hullHealth.CurrentValue == 0)
                        NetworkServer.Destroy(collision.gameObject);
                }
            }

            NetworkServer.Destroy(gameObject);
        }
    }
}

