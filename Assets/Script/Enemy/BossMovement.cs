using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BossMovement : EnemyBase
    {
        private enum AttackPattern
        {
            Charging
        }
        private enum ChargingState
        {
            ChargingUp,
            Moving,

        }

        private AttackPattern currentAttactPattern;
        private ChargingState currentChargingState;
        private Vector3 chargingTargetPosition;
        [SerializeField] [Min(1)] private float timeBetweenCharges; 

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
                case enemyState.Attacking:
                    UpdateAttackPattern();
                    break;
            }
        }

        private void UpdateAttackPattern()
        {
            switch (currentAttactPattern)
            {
                case AttackPattern.Charging:
                    UpdateChasingPattern();
                    break;
            }
        }

        private void UpdateChasingPattern()
        {
            switch (currentChargingState)
            {
                case ChargingState.ChargingUp:
                    break;
                case ChargingState.Moving:
                    break;

            }
        }

        new protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < attackingRange) currentState = enemyState.Attacking;
            else currentState = enemyState.Patrolling;
        }
    }
}
