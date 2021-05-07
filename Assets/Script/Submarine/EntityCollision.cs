using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class EntityCollision : NetworkBehaviour
    {
        private ShipResource hullHealth;
        private bool npc;

        private void Start()
        {
            hullHealth = GetComponent<ShipResource>();
            npc = CompareTag(ReferenceManager.Singleton.EnemyTag);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("EnemyBullet") || collision.CompareTag("AllyBullet"))
            {
                if ((!npc && collision.CompareTag("EnemyBullet")) || (npc && collision.CompareTag("AllyBullet")))
                    hullHealth.ApplyChange(-collision.gameObject.GetComponent<Bullet>().Damage);
                Destroy(collision.gameObject);
            }
            else if (!npc && collision.CompareTag(ReferenceManager.Singleton.EnemyTag)) //If i'm not an npc and i collide with an enemy ship
                hullHealth.ApplyChange(-collision.gameObject.GetComponentInParent<EnemyBase>().CollisionDamage);
        }
    }
}
