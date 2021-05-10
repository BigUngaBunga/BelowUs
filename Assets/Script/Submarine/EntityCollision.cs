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
            if (!npc && collision.CompareTag(ReferenceManager.Singleton.EnemyTag)) //If i'm not an npc and i collide with an enemy ship
                hullHealth.ApplyChange(-collision.gameObject.GetComponentInParent<EnemyBase>().CollisionDamage);
        }
    }
}
