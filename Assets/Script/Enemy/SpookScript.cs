using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class SpookScript : EnemyBase
    {
        private Weapon weapon;
        private Transform firePoint;
        private float timeElapsed = 0;

        [SerializeField] private float timeBetweenShoots;

        protected override void Start()
        {
            weapon = this.GetComponent<Weapon>();
            rb = this.GetComponent<Rigidbody2D>();
            firePoint = gameObject.transform.Find("FirePoint");

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
                    UpdateRotationPatrolling();
                    UpdateMovementPatrolling();
                    base.CheckForFlip();
                    break;
                case enemyState.Chasing:
                    UpdateRotationChasing();
                    UpdateMovementChasing();
                    base.CheckForFlip();
                    break;
                case enemyState.Attacking:
                    UpdateRotationAttacking();
                    UpdateFirePointRotation();
                    timeElapsed += Time.deltaTime;
                    if (timeElapsed >= timeBetweenShoots)
                    {
                        timeElapsed -= timeBetweenShoots;
                        weapon.shoot();
                    }
                    break;
            }
        }

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

        protected void UpdateRotationAttacking()
        {
            //Uträkningen av direction är omvänd för att fisken ska rotera 180 grader så att baksidan är roterad mot submarine
            Vector2 direction = transform.position - targetGameObject.transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));
        }

        protected void UpdateFirePointRotation()
        {
            //Används endast så att skoten skjuts i rätt riktning pga av UpdateRotationAttacking
            Vector2 direction = targetGameObject.transform.position - firePoint.transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            firePoint.rotation = (Quaternion.Slerp(firePoint.transform.rotation, rotation, Time.deltaTime * 5f));
        }
    }

}
