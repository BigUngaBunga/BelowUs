using UnityEngine;

namespace BelowUs
{
    public class SubmarineBullet : MonoBehaviour
    {
        private Rigidbody2D rb;
        private float speed, timer;
        private int damage;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            speed = 5;
            rb.velocity = -transform.up * speed;
            damage = 20;
        }

        void Update() => BulletTime();
        private void BulletTime()
        {
            timer += Time.deltaTime;
            if (timer >= 6)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                other.GetComponent<EnemyBase>().TakeDamage(damage);
            }
        }
    }
}