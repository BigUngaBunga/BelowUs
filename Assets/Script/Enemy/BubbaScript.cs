using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class BubbaScript : EnemyBase
    {

        private Rigidbody2D rb;
        //private float maxSpeed = 0.1f;
        public float movementSpeed = 5f;

        protected override void Start()
        {
            rb = this.GetComponent<Rigidbody2D>();
            moveSpeedPatrolling = 5f;
            moveSpeedChasing = 7f;
            targetName = "Gameobject";
            currentState = enemyState.Patrolling;
            CreatePatrolArea();
            SetTarget();
            GetNextPatrolPosition();
        }

        protected override void Update()
        {
            switch (currentState)
            {
                case enemyState.Patrolling:
                    UpdateRotationPatrolling();
                    UpdateMovementPatrolling();
                    CheckForFlip();

                    if (Vector3.Distance(targetGameObject.transform.position, transform.position) < 50f)
                    {
                        currentState = enemyState.Chasing;
                    }

                    break;
                case enemyState.Chasing:
                    UpdateRotationChasing();
                    UpdateMovementChasing();
                    break;
            }
        }

        #region chasing

        //gammal Rotation
        //protected void UpdateRotationChasing()
        //{
        //    Vector3 direction = currentPatrolTarget - transform.position;
        //    direction = targetGameObject.transform.position - transform.position;
        //    float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        //    rb.rotation = -angle;
        //    direction.Normalize();
        //}
        //Add force movement 
        //protected void UpdateMovementChasing()
        //{
        //    Vector2 direction = (TargetLocation.position - transform.position).normalized;
        //    float DistanceMultiplier = 20 - Vector2.Distance(TargetLocation.position, transform.position);
        //    Vector2 movement = direction * movementSpeed * Time.deltaTime * DistanceMultiplier;


        //    rb.AddForce(movement);

        //    if (Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
        //    {
        //        base.GetNextPatrolPosition();
        //    }
        //}
        //Ny movement
        //protected void UpdateMovementPatrolling()
        //{
        //    Vector2 direction = (currentPatrolTarget - transform.position).normalized;
        //    Vector2 movement = direction * movementSpeed * Time.deltaTime;

        //     rb.AddForce(movement);

        //    if (Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
        //    {
        //        base.GetNextPatrolPosition();
        //    }
        //}

        protected void UpdateMovementChasing()
        {
            Vector2 direction = targetGameObject.transform.position - transform.position;
            rb.MovePosition((Vector2)transform.position + (direction * moveSpeedChasing * Time.deltaTime));
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

        void CheckForFlip()
        {
            //Checks which direction the objeckt is facing and wether it has flipped the right way thru localscale
            if (transform.rotation.z > 0 && transform.localScale.x < 0)
            {
                flip();
            }
            else if (transform.rotation.z < 0 && transform.localScale.x > 0)
            {
                flip();
            }
        }
        void flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }


        #endregion patrolling
    }

}