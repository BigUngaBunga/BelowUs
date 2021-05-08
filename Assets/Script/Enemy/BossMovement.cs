using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BossMovement : EnemyBase
    {
        protected enum EnemyState
        {
            Patrolling,
            Attacking
        }
        private enum ChargingState
        {
            ChargingUp,
            MovingChasing,
            Relocate,
            MovingRelocate
        }
        private enum AttackPattern
        {
            Charging
        }

        [SerializeField] private float chasingDistanceBeforeNewTarget;
        [SerializeField] private float chasingRotationSpeed;
        [SerializeField] [Min(1)] private float timeBetweenCharges;
        [SerializeField] protected EnemyState currentState;
        [SerializeField] [Min(10)]protected float degressToStartCharging;

        private Vector3 chargingTargetPosition;
        private ChargingState currentChargingState;
        private AttackPattern currentAttactPattern;
        private float timeElapsed;

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
                case EnemyState.Attacking:
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
                    if (ChasingRotation(chargingTargetPosition, chasingRotationSpeed))
                    {
                        timeElapsed += Time.deltaTime;
                        if (timeElapsed >= timeBetweenCharges)
                        {
                            timeElapsed = 0;
                            currentChargingState = ChargingState.MovingChasing;
                        }
                    }
                    break;

                case ChargingState.MovingChasing:
                    ChasingMovement();
                    if (Vector3.Distance(transform.position, chargingTargetPosition) < chasingDistanceBeforeNewTarget)
                    {
                        chargingTargetPosition = ChasingNewTarget();
                        currentChargingState = ChargingState.Relocate;
                    }
                    break;

                //Behöver snacka med truls om att fixa bättre position
                case ChargingState.Relocate:
                    chargingTargetPosition = transform.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20));
                    currentChargingState = ChargingState.MovingRelocate;
                    break;

                case ChargingState.MovingRelocate:
                    ChasingMovement();
                    if (Vector3.Distance(transform.position, chargingTargetPosition) < chasingDistanceBeforeNewTarget)
                    {
                        rb.velocity = new Vector2(0, 0);
                        rb.angularVelocity = 0f;
                        chargingTargetPosition = ChasingNewTarget();
                        currentChargingState = ChargingState.ChargingUp;
                    }
                    break;

            }
        }

        protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < attackingRange) currentState = EnemyState.Attacking;
            else currentState = EnemyState.Patrolling;
        }



        #region chasing Attack pattern

        private Vector3 ChasingNewTarget() => targetGameObject.transform.position;

        protected void ChasingMovement()
        {
            Vector2 direction = (chargingTargetPosition - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        protected bool ChasingRotation(Vector3 targetPosition, float rotationSpeed)
        {
            Vector2 direction = targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            //if within 10* begin charging
            float angleDistance = Quaternion.Angle(rotation, transform.rotation);
            if (angleDistance < degressToStartCharging) return true;
            else return false;
        }
        #endregion
    }
}
