using UnityEngine;

namespace BelowUs
{
    public class SubmarineBullet : MonoBehaviour
    {
        private Rigidbody2D rb;
        private float speed, timer;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            Vector2 dir = new Vector2(transform.forward.x, transform.forward.y).normalized;
            speed = 5;
            rb.velocity = dir * speed;
        }

        void Update()
        {
            BulletTime();
        }

        private void BulletTime()
        {
            timer += Time.deltaTime;
            if (timer >= 6)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Enemy")
            {
                Destroy(gameObject);
                other.GetComponent<EnemyBase>().TakeDamage(20);
            }
        }
    }
}