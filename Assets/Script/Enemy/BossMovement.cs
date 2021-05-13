using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BossMovement : EnemyBase
    {
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
        [SerializeField] [Min(10)]protected float degressToStartCharging;

        private Vector3 chargingTargetPosition;
        private ChargingState currentChargingState;
        private AttackPattern currentAttactPattern;
        private float timeElapsed;

        

        [Server]
        protected override void Update()
        {
            CheckDistanceToTargetChasing();

            switch (currentState)
            {
                case EnemyState.Idle:                    
                    break;
                case EnemyState.Chasing:
                    UpdateAttackPattern();
                    break;
            }
            CheckForFlip();
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

                //Beh�ver snacka med truls om att fixa b�ttre position
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

            //if within x degress begin charging
            float angleDistance = Quaternion.Angle(rotation, transform.rotation);
            if (angleDistance < degressToStartCharging) return true;
            else return false;
        }
        #endregion
    }
}