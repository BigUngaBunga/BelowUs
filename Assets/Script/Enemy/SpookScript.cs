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


        protected void UpdateRotationAttacking()
        {
            //Utr�kningen av direction �r omv�nd f�r att fisken ska rotera 180 grader s� att baksidan �r roterad mot submarine
            Vector2 direction = transform.position - targetGameObject.transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));
        }

        protected void UpdateFirePointRotation()
        {
            //Anv�nds endast s� att skoten skjuts i r�tt riktning pga av UpdateRotationAttacking
            Vector2 direction = targetGameObject.transform.position - firePoint.transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            firePoint.rotation = (Quaternion.Slerp(firePoint.transform.rotation, rotation, Time.deltaTime * 5f));
        }
    }

}
