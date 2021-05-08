using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class SpookScript : EnemyBase
    {
        private enum EnemyState
        {
            Patrolling,
            Chasing,
            Attacking
        }

        private Weapon weapon;
        private Transform firePoint;
        private float timeElapsed = 0;

        [SerializeField] private float timeBetweenShoots;       
        [SerializeField] private EnemyState currentState;


        protected override void Start()
        {
            weapon = GetComponent<Weapon>();
            rb = GetComponent<Rigidbody2D>();
            firePoint = gameObject.transform.Find("FirePoint");

            CreatePatrolArea();
            SetTarget();
            GetNextPatrolPosition();
        }

        protected override void Update() 
        {
            //TODO don't even run this if it's not the server
            //maybe use invokerepeating with a low delay or something else
            if (isServer)
                NewUpdate();
        }

        [Server]
        private void NewUpdate()
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
                case EnemyState.Attacking:
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

        protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < attackingRange) currentState = EnemyState.Attacking;
            else if (distance < chasingRange) currentState = EnemyState.Chasing;
            else currentState = EnemyState.Patrolling;
        }

        protected void UpdateMovementChasing()
        {
            Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
            Vector2 movement = direction * moveSpeedChasing * Time.deltaTime;
            rb.AddForce(movement);
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
