using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class EnemyBase : NetworkBehaviour
    {
        protected enum EnemyState
        {
            Patrolling,
            Chasing,
            Attacking
        }

        [SerializeField] protected float chasingRange, attackingRange;
        [SerializeField] protected float collisionDamage;

        public float CollisionDamage => collisionDamage;



        protected string targetName;

        [SerializeField] [Min(10)] protected float moveSpeedPatrolling;
        [SerializeField] [Min(10)] protected float moveSpeedChasing;
        [SerializeField] [Min(1)] protected float maxPatrolRange;


        [SerializeField] protected GameObject targetGameObject;
        protected List<Vector3> patrolPositions = new List<Vector3>();
        protected Vector3 currentPatrolTarget;

        protected EnemyState currentState;
        protected Rigidbody2D rb;
        [SerializeField] protected ShipResource hullHP;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            hullHP = GetComponent<ShipResource>();
            hullHP.EventResourceEmpty += Die;
        }

        protected virtual void Update()
        {

        }


        protected void SetTarget() => targetGameObject = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.SubmarineTag);

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

        protected void CheckForFlip()
        {
            //Checks which direction the objeckt is facing and wether it has flipped the right way thru localscale
            if (transform.rotation.z > 0 && transform.localScale.x < 0) Flip();
            else if (transform.rotation.z < 0 && transform.localScale.x > 0) Flip();
        }

        protected void Flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        protected void CheckDistanceToTarget()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < attackingRange) currentState = EnemyState.Attacking;
            else currentState = distance < chasingRange ? EnemyState.Chasing : EnemyState.Patrolling;
        }

        private void Die() => Destroy(gameObject);
    }
}

