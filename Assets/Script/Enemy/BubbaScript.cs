using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BubbaScript : EnemyBase
    {
        protected enum EnemyState
        {
            Patrolling,
            Chasing          
        }
        [SerializeField] protected EnemyState currentState;
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


        private void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        #endregion chasing


        private void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);            
            if (distance < chasingRange) currentState = EnemyState.Chasing;
            else currentState = EnemyState.Patrolling;
        }
    }
}
