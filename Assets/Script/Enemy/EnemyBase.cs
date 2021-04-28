using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class EnemyBase : NetworkBehaviour
    {
        protected enum enemyState
        {
            Patrolling,
            Chasing,
            Attacking
        }


        [SerializeField] protected float chasingRange, attackingRange;
        [SerializeField] protected float health;
        [SerializeField] protected float collisionDamage;

        public float CollisionDamage => collisionDamage;



        protected string targetName;

        [SerializeField] [Min(10)] protected float moveSpeedPatrolling;
        [SerializeField] [Min(10)] protected float moveSpeedChasing;
        [SerializeField] [Min(1)] protected float maxPatrolRange;


        [SerializeField] protected GameObject targetGameObject;
        protected List<Vector3> patrolPositions = new List<Vector3>();
        protected Vector3 currentPatrolTarget;

        protected enemyState currentState;
        protected Rigidbody2D rb;



        [TagSelector] [SerializeField] private string submarineTag;
        public string SubmarineTag => submarineTag;

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }


        protected void SetTarget() => targetGameObject = GameObject.FindGameObjectWithTag(submarineTag);

        private void AddRandomPatrolNumber() => patrolPositions.Add(new Vector3(Random.Range(-maxPatrolRange, maxPatrolRange), Random.Range(-maxPatrolRange, maxPatrolRange)));

        protected void CreatePatrolArea()
        {
            for (int i = 0; i < 4; i++)
            {
                AddRandomPatrolNumber();
            }
        }

        protected void GetNextPatrolPosition()
        {
            currentPatrolTarget = patrolPositions[0];
            patrolPositions.RemoveAt(0);
            AddRandomPatrolNumber();
        }

        protected void CheckIfAlive()
        {
            if (health <= 0) Destroy(gameObject);
        }

        protected void CheckForFlip()
        {
            //Checks which direction the objeckt is facing and wether it has flipped the right way thru localscale
            if (transform.rotation.z > 0 && transform.localScale.x < 0) flip();
            else if (transform.rotation.z < 0 && transform.localScale.x > 0) flip();
        }

        protected void flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < attackingRange) currentState = enemyState.Attacking;
            else if (distance < chasingRange) currentState = enemyState.Chasing;
            else currentState = enemyState.Patrolling;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "AllyBullet")
            {
                health -= collision.gameObject.GetComponent<Bullet>().Damage;
            }
            CheckIfAlive();
        }
    }
}

