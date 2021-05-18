using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BossMovement : EnemyBase
    {
        private enum OldChargingState
        {
            ChargingUp,
            MovingChasing,
            Relocate,
            MovingRelocate
        }

        private enum ChargingState
        {
            Entry,
            Charge,
            TargetReached
        }


        private enum AttackPattern
        {
            OldCharging,
            Charging,
            BasicChasing,
        }

        [SerializeField] private float chasingDistanceBeforeNewTarget;
        [SerializeField] private float chasingRotationSpeed;
        [SerializeField] [Min(1)] private float timeBetweenCharges;
        [SerializeField] [Min(10)] protected float degressToStartCharging;
        [SerializeField] private Vector2 centerPosition;




        //Debug
        private float debugRotate = 0;
        private float debugTimeElapsed = 0;
        [SerializeField] private float debugTimeBetween = 0.2f;
        [SerializeField] private bool debug;



        //Old Charge
        private Vector3 oldChargingTargetPosition;
        private OldChargingState oldCurrentChargingState;

        //New Charge
        [SerializeField] private ChargingState currentChargingState;
        [SerializeField] [Min(40)] private int distanceFromCenterPointCharging;
        private Vector2 chargingTargetPosition;
        float angle;

        //Behöver lägga till flera patterns och ett sätt att bestämma dom, Nedan ska ej hardcodas
        private AttackPattern currentAttactPattern = AttackPattern.Charging;


        private float timeElapsed;

        protected override void Start()
        {
            centerPosition = transform.position;

            base.Start();
        }

        protected override void Update()
        {
            if (isServer)
                UpdateStates();

        }

        [Server]
        private void UpdateStates()
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
                case AttackPattern.BasicChasing:
                    BasicChasing();
                    break;
                case AttackPattern.OldCharging:
                    UpdateOldChasingPattern();
                    break;
                case AttackPattern.Charging:
                    //SetFirstPosition();
                    UpdateCharging();
                    break;
            }
        }

        #region Charging

        private void UpdateCharging()
        {
            DebugPositining();

            switch (currentChargingState)
            {                
                case ChargingState.Entry: //Sets first location
                    chargingTargetPosition = new Vector2(centerPosition.x + distanceFromCenterPointCharging, centerPosition.y);
                    currentChargingState = ChargingState.Charge;
                    break;               
                case ChargingState.Charge:
                    UpdateMovementCharging();

                    if (Vector3.Distance(transform.position, chargingTargetPosition) < chasingDistanceBeforeNewTarget)
                    {
                        currentChargingState = ChargingState.TargetReached;
                        rb.MovePosition(chargingTargetPosition);
                    }
                    break;
                case ChargingState.TargetReached:
                    FindNextTargetPosition();
                    currentChargingState = ChargingState.Charge;
                    break;
            }
        }

        protected void UpdateMovementCharging()
        {
            Vector2 direction = (chargingTargetPosition - (Vector2)transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        private void FindNextTargetPosition()
        {
            Vector2 normalizedDirection = ((Vector2)targetGameObject.transform.position - (Vector2)transform.position).normalized;
            float angle = Vector2.Angle(targetGameObject.transform.position - transform.position, centerPosition - (Vector2)transform.position);
            float distance = Mathf.Cos(Mathf.Deg2Rad * angle) * Vector2.Distance((Vector2)transform.position, centerPosition) * 2;

            chargingTargetPosition = ((Vector2)transform.position + normalizedDirection * distance);
        }

        private void DebugPositining()
        {
            if (debug)
            {
                Debug.DrawLine(transform.position, chargingTargetPosition, Color.red);
                Debug.DrawLine(transform.position, targetGameObject.transform.position);
                Debug.DrawLine(centerPosition, targetGameObject.transform.position);
            }
            if (false) // extra debug
            {
                debugTimeElapsed += Time.deltaTime;
                if (debugTimeElapsed > debugTimeBetween)
                {
                    debugTimeElapsed -= debugTimeBetween;
                    debugRotate += 0.01f;
                    rb.MovePosition(new Vector2(centerPosition.x + Mathf.Cos(debugRotate) * 40, centerPosition.y + Mathf.Sin(debugRotate) * 40));
                }
            }
        }

        #endregion

        #region Basic Chaasing

        private void BasicChasing()
        {
            UpdateBasicRotation(targetGameObject.transform.position);
            UpdateMovementChasing();
        }

        protected void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }

        #endregion

        #region Old Chasing       

        private void UpdateOldChasingPattern()
        {
            switch (oldCurrentChargingState)
            {
                case OldChargingState.ChargingUp:
                    if (ChasingRotation(oldChargingTargetPosition, chasingRotationSpeed))
                    {
                        timeElapsed += Time.deltaTime;
                        if (timeElapsed >= timeBetweenCharges)
                        {
                            timeElapsed = 0;
                            oldCurrentChargingState = OldChargingState.MovingChasing;
                        }
                    }
                    break;

                case OldChargingState.MovingChasing:
                    ChasingMovement();
                    if (Vector3.Distance(transform.position, oldChargingTargetPosition) < chasingDistanceBeforeNewTarget)
                    {
                        oldChargingTargetPosition = ChasingNewTarget();
                        oldCurrentChargingState = OldChargingState.Relocate;
                    }
                    break;

                case OldChargingState.Relocate:
                    oldChargingTargetPosition = transform.position + new Vector3(Random.Range(-20, 20), Random.Range(-20, 20));
                    oldCurrentChargingState = OldChargingState.MovingRelocate;
                    break;

                case OldChargingState.MovingRelocate:
                    ChasingMovement();
                    if (Vector3.Distance(transform.position, oldChargingTargetPosition) < chasingDistanceBeforeNewTarget)
                    {
                        rb.velocity = new Vector2(0, 0);
                        rb.angularVelocity = 0f;
                        oldChargingTargetPosition = ChasingNewTarget();
                        oldCurrentChargingState = OldChargingState.ChargingUp;
                    }
                    break;

            }
        }

        private Vector3 ChasingNewTarget() => targetGameObject.transform.position;

        protected void ChasingMovement()
        {
            Vector2 direction = (oldChargingTargetPosition - transform.position).normalized;
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

        private void CheckDistanceToTargetBossMovement()
        {

        }

    }
}
