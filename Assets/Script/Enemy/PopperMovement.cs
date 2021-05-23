using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class PopperMovement : EnemyBase
    {
               
        [SerializeField] private float explosionRange;
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionDamage;

        
        protected override void Update()
        {
            if (isServer)
                ServerStuff();
        }

        [Server]
        private void ServerStuff()
        {
            CheckDistanceToTargetChasing();

            switch (currentState)
            {
                case EnemyState.Idle:
                    
                    break;
                case EnemyState.Chasing:
                    UpdateBasicRotation(targetGameObject.transform.position);
                    UpdateMovementChasing();
                    break;
               
            }
            CheckForFlip();

        }

        #region chasing
        protected void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        #endregion chasing       

        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Submarine"))
            {
                Explode();
            }
        }


        private void Explode()
        {
            //Instatiate Explosion here

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRange);

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Resource")){
                    Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
                    AddExplosionForce(rb);
                }
                else if (collider.gameObject.CompareTag("Submarine") && !collider.isTrigger){ //IsTrigger används eftersom sub har två colliders
                    Rigidbody2D rb = collider.gameObject.GetComponentInParent<Rigidbody2D>();
                    AddExplosionForce(rb);

                    AddExplosionDamage(collider);
                }
            }

            NetworkServer.Destroy(gameObject);
        }

        private void AddExplosionForce(Rigidbody2D rb)
        {
            Vector2 explosionDir = rb.position - (Vector2)transform.position;
            rb.AddForce(explosionDir * explosionForce);
        }
       
        private void AddExplosionDamage(Collider2D collider) => collider.gameObject.GetComponentInParent<ShipResource>().ApplyChange(explosionDamage);
    }
}
