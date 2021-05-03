using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BubbaScript : EnemyBase
    {
        protected override void Start()
        {
            rb = this.GetComponent<Rigidbody2D>();

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
                case enemyState.Patrolling:
                    UpdateBasicRotation(currentPatrolTarget);
                    UpdateMovementPatrolling();
                    CheckForFlip();
                    break;
                case enemyState.Chasing:
                    UpdateBasicRotation(targetGameObject.transform.position);
                    UpdateMovementChasing();
                    CheckForFlip();
                    break;
                case enemyState.Attacking:
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
    }
}
