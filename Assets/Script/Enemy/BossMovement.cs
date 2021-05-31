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
            Charge          
        }


        private enum AttackPattern
        {
            Charging,
            BasicChasing,
        }

        [SerializeField] private float chasingDistanceBeforeNewTarget;
        [SerializeField] private float chasingRotationSpeed;
        [SerializeField] [Min(1)] private float timeBetweenCharges;
        [SerializeField] [Min(10)] protected float degressToStartCharging;
        [SerializeField] private Vector2 centerPosition;

        //targeting wont be longer than the size of the room - offsetDistanceChasingRange
        [SerializeField] private int offsetDistanceChasingRange;

        //Debug        
        [SerializeField] private bool debug;

        
        //Charge
        [SerializeField] private ChargingState currentChargingState;
        [SerializeField] [Min(40)] private int distanceFromCenterPointCharging;        
        private Vector2 normalizedDirection;

        //Behöver lägga till flera patterns och ett sätt att bestämma dom, Nedan ska ej hardcodas
        private AttackPattern currentAttactPattern = AttackPattern.BasicChasing;       

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
            UpdateBasicRotation(targetGameObject.transform.position);

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
            CheckIfSubIsPastChargingArea();

            switch (currentAttactPattern)
            {
                case AttackPattern.BasicChasing:
                    BasicChasing();
                    break;                
                case AttackPattern.Charging:
                    UpdateCharging();
                    break;
            }
        }

        #region Charging

        private void UpdateCharging()
        {
            switch (currentChargingState)
            {                
                case ChargingState.Entry: //Sets first location
                    currentChargingState = ChargingState.Charge;
                    FindNextTargetPosition();
                    break;               
                case ChargingState.Charge:
                    UpdateMovementCharging();
                    CheckifBossIsPassCircle();                    
                    break;                
            }
        }

        protected void UpdateMovementCharging()
        {
            Vector2 movement = normalizedDirection * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
        }
        
        private void FindNextTargetPosition()
        {
            Vector2 unnormalizedDirection = ((Vector2)targetGameObject.transform.position - (Vector2)transform.position);
            normalizedDirection = new Vector2(unnormalizedDirection.x / unnormalizedDirection.magnitude, unnormalizedDirection.y / unnormalizedDirection.magnitude);                       
        }

        private void CheckIfSubIsPastChargingArea()
        {
            if(Vector2.Distance(targetGameObject.transform.position, centerPosition) > distanceFromCenterPointCharging)
            {
                currentAttactPattern = AttackPattern.BasicChasing;
            }
            else if(Vector2.Distance(targetGameObject.transform.position, centerPosition) < distanceFromCenterPointCharging)
            {
                currentAttactPattern = AttackPattern.Charging;
            }
        }

        private void CheckifBossIsPassCircle()
        {
            if(Vector2.Distance(transform.position, centerPosition ) > distanceFromCenterPointCharging)
            {
                FindNextTargetPosition();
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
        
        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(centerPosition, distanceFromCenterPointCharging);
            }
        }

        public void SetTargetingOffsetRange(float widthOfRoom) => chasingRange = widthOfRoom - offsetDistanceChasingRange;
        //targeting wont be longer than the size of the room - offsetDistanceChasingRange

    }
}
