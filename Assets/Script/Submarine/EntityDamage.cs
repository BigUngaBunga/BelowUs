using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class EntityDamage : NetworkBehaviour
    {
        [SerializeField] private ShipResource hullHealth;

        private void Start() => hullHealth = GameObject.Find("Hull Health").GetComponent<ShipResource>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("EnemyBullet"))

                hullHealth.ApplyChange(collision.gameObject.GetComponent<Bullet>().Damage);

            else if (collision.CompareTag("Enemy"))

                hullHealth.ApplyChange(collision.gameObject.GetComponentInParent<EnemyBase>().CollisionDamage);
        }
    }
}
