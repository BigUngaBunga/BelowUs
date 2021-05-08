using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class EntityCollision : NetworkBehaviour
    {
        private ShipResource hullHealth;
        [SerializeField] private bool npc;
        [SerializeField] private bool debug;

        private void Start()
        {
            hullHealth = GetComponent<ShipResource>();
            npc = CompareTag(ReferenceManager.Singleton.EnemyTag);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("EnemyBullet") || collision.CompareTag("AllyBullet"))
            {
                bool alliedBullet = (npc && collision.CompareTag("EnemyBullet")) || (!npc && collision.CompareTag("AllyBullet"));
                
                if (debug)
                {
                    string info = transform.name + " was hit by a ";
                    if (alliedBullet)
                        Debug.Log(info + nameof(alliedBullet));
                    else
                        Debug.Log(info + "enemy bullet");
                }

                if (!alliedBullet)
                {
                    float damage = collision.gameObject.GetComponent<Bullet>().Damage;

                    if (debug)
                        Debug.Log("Damage is: " + damage + "\nHealth before: " + hullHealth.CurrentValue);
                    
                    hullHealth.ApplyChange(-damage);
                    
                    if (debug)
                        Debug.Log("Health after: " + hullHealth.CurrentValue);

                    if (npc && hullHealth.CurrentValue == 0)
                        Destroy(gameObject);
                }
                    
                Destroy(collision.gameObject);
            }
            else if (!npc && collision.CompareTag(ReferenceManager.Singleton.EnemyTag)) //If i'm not an npc and i collide with an enemy ship
                hullHealth.ApplyChange(-collision.gameObject.GetComponentInParent<EnemyBase>().CollisionDamage);
        }
    }
}
