using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SubmarineDamage : NetworkBehaviour
{
    private ShipResource hullHealth;

    private void Start()
    {
        hullHealth = GameObject.Find("Hull Health").GetComponent<ShipResource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyBullet")

            hullHealth.ApplyChange(collision.gameObject.GetComponent<Bullet>().Damage);

        else if (collision.tag == "Enemy")

            hullHealth.ApplyChange(collision.gameObject.GetComponentInParent<EnemyBase>().CollisionDamage);
    }
}
