using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class PopperMovement : EnemyBase
    {
        protected enum EnemyState
        {
            Patrolling,
            Chasing           
        }

        [SerializeField] protected EnemyState currentState;
        [SerializeField] private float explosionRange;
        [SerializeField] private float explosionForce;
        [SerializeField] private float explosionDamage;

        protected override void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            CreatePatrolArea();
            SetTarget();
            GetNextPatrolPosition();
        }

        [Server]
        protected override void Update()
        {
            CheckDistanceToTarget();

            switch (currentState)
            {
                case EnemyState.Patrolling:
                    UpdateBasicRotation(currentPatrolTarget);
                    UpdateMovementPatrolling();
                    CheckForFlip();
                    break;
                case EnemyState.Chasing:
                    UpdateBasicRotation(targetGameObject.transform.position);
                    UpdateMovementChasing();
                    CheckForFlip();
                    break;
               
            }
        }

        #region chasing
        protected void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        #endregion chasing

        protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < chasingRange) currentState = EnemyState.Chasing;
            else currentState = EnemyState.Patrolling;
        }

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
                    Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
                    AddExplosionForce(rb);
                    AddExplosionDamage();
                }
            }

            Destroy(gameObject);
        }

        private void AddExplosionForce(Rigidbody2D rb)
        {
            Vector2 explosionDir = rb.position - (Vector2)transform.position;
            rb.AddForce(explosionDir * explosionForce);
        }

        private void AddExplosionDamage() => GameObject.Find("Hull Health").GetComponent<ShipResource>().ApplyChange(explosionDamage);
    }
}
