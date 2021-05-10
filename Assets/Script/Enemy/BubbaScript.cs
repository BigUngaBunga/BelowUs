using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class BubbaScript : EnemyBase
    {
        protected override void Start()
        {
            base.Start();

            CreatePatrolArea();
            SetTarget();
            GetNextPatrolPosition();
        }
        
        protected override void Update()
        {
            if (isServer)
                ServerStuff();
        }

        [Server]
        private void ServerStuff()
        {
            CheckDistanceToTarget();

            switch (currentState)
            {
                case EnemyState.Patrolling:
                    UpdateRotationPatrolling();
                    UpdateMovementPatrolling();
                    break;
                default:
                    UpdateRotationChasing();
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
        protected void UpdateRotationChasing()
        {
            Vector2 direction = targetGameObject.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));

        }
        #endregion chasing

        #region patrolling

        protected void UpdateRotationPatrolling()
        {
            //Vector till target
            Vector2 direction = currentPatrolTarget - transform.position;
            //vinkel till target
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            //rotationen som krävs till target som en quaternion runt z axlen
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            //Mindre del av rotationen till target (slerp)
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));
        }

        protected void UpdateMovementPatrolling()
        {
            Vector3 direction = (currentPatrolTarget - transform.position);
            rb.MovePosition(transform.position + (direction * moveSpeedPatrolling * Time.deltaTime));

            if (Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
            {
                base.GetNextPatrolPosition();
            }
        }

        #endregion patrolling   
    }

}
