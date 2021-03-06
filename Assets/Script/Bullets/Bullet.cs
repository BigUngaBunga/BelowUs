using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CapsuleCollider2D capsuleCollider;

        [SerializeField] private float velocity = 20;
        [SerializeField] private FloatVariable cannonDamage;
        [SerializeField] private float damage;
        [SerializeField] private int expirationTime;
        [SerializeField] private bool invertDirection;

        [SerializeField] private float collisionDelay = 0.05f;
        [SerializeField] private bool npc;
        private AudioSource audioSource;

        public float Damage => cannonDamage == null ? damage : cannonDamage.Value;

        private readonly bool debugProjectile = false;
        private readonly bool debugCollision = false;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (isServer)
            {
                int invertDirectionInt = invertDirection ? -1 : 1;

                Vector2 yVelocity = invertDirectionInt * velocity * transform.up;

                if (debugProjectile)
                    Debug.Log("Initial Velocity " + rb.velocity);

                rb.velocity = yVelocity;

                if (debugProjectile)
                    Debug.Log("Velocity after " + nameof(yVelocity) + " " + rb.velocity);

                npc = CompareTag("EnemyBullet");

                Invoke(nameof(EnableCollision), collisionDelay); //This prevents the bullet from colliding with it's creator.
                Invoke(nameof(Expire), expirationTime);
            }
        }

        /**
         * Activates bullet collision.
         */
        private void EnableCollision() => capsuleCollider.enabled = true;

        [Server] private void Expire() => NetworkServer.Destroy(gameObject);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(ReferenceManager.Singleton.SubmarineTag) || collision.CompareTag(ReferenceManager.Singleton.EnemyTag))
            {
                bool alliedShip = (npc && collision.CompareTag(ReferenceManager.Singleton.EnemyTag)) || (!npc && collision.CompareTag(ReferenceManager.Singleton.SubmarineTag));

                if (debugCollision)
                {
                    string info = transform.name + " was hit by a ";
                    if (alliedShip)
                        Debug.Log(info + nameof(alliedShip));
                    else
                        Debug.Log(info + "enemy bullet");
                }

                if (!alliedShip)
                {
                    //TODO fix Bubba's health so that it's not null
                    //Todo change this if collider position is standardized
                    ShipResource health = collision.gameObject.GetComponent<ShipResource>();

                    if (health == null)
                        health = collision.gameObject.GetComponentInParent<ShipResource>();

                    if (debugCollision && health == null)
                        Debug.Log("Health is null");

                    if (debugCollision)
                        Debug.Log("Damage is: " + damage + "\nHealth before: " + health.CurrentValue);

                    health.ApplyChange(-Damage);

                    if (debugCollision)
                        Debug.Log("Health after: " + health.CurrentValue);

                    if (npc && health.CurrentValue == 0)
                        NetworkServer.Destroy(collision.gameObject);
                }
            }

            if (isServer)
                ServerPlaySound();
            else
                CommandPlaySound();

            NetworkServer.Destroy(gameObject);
        }

        [Server]
        private void ServerPlaySound() => PlaySound();

        [Command(requiresAuthority = false)]
        private void CommandPlaySound() => ServerPlaySound();

        [ClientRpc]
        private void PlaySound() => AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }
}

