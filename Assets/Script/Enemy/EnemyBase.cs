using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace BelowUs
{
    public class EnemyBase : NetworkBehaviour
    {
        


        [SerializeField] protected float chasingRange, attackingRange;
        [SerializeField] protected float health;
        [SerializeField] protected float collisionDamage;

        public float CollisionDamage => collisionDamage;



        protected string targetName;

        [SerializeField] [Min(1)] protected float moveSpeedPatrolling;
        [SerializeField] [Min(1)] protected float moveSpeedChasing;
        [SerializeField] [Min(1)] protected float maxPatrolRange;


        [SerializeField] protected GameObject targetGameObject;
        protected List<Vector3> patrolPositions = new List<Vector3>();
        protected Vector3 currentPatrolTarget;

        protected Rigidbody2D rb;

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }


        protected void SetTarget() => targetGameObject = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.SubmarineTag);

        private void AddRandomPatrolNumber() => patrolPositions.Add(new Vector3(Random.Range(-maxPatrolRange, maxPatrolRange), Random.Range(-maxPatrolRange, maxPatrolRange)) + transform.position);

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
            if (transform.rotation.z > 0 && transform.localScale.x < 0) Flip();
            else if (transform.rotation.z < 0 && transform.localScale.x > 0) Flip();
        }

        protected void Flip()
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "AllyBullet")
            {
                health -= collision.gameObject.GetComponent<Bullet>().Damage;
            }
            CheckIfAlive();
        }        

        protected void UpdateMovementPatrolling()
        {
            Vector3 direction = (currentPatrolTarget - transform.position);
            rb.MovePosition(transform.position + (direction * moveSpeedPatrolling * Time.deltaTime));

            if (Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
            {
                GetNextPatrolPosition();
            }
        }

        protected void UpdateBasicRotation(Vector3 targetPosition)
        {
            //Vector till target
            Vector2 direction = targetPosition - transform.position;
            //vinkel till target
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            //rotationen som krävs till target som en quaternion runt z axlen
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            //Mindre del av rotationen till target (slerp)
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));
        }
        
    }
}

