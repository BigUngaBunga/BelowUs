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
            Idle,
            Chasing,
            Firing
        }

        [SerializeField] protected float chasingRange, FiringRange;
        [SerializeField] protected float collisionDamage;

        public float CollisionDamage => collisionDamage;
        [SerializeField] [Min(10)] protected float moveSpeedChasing;


        protected GameObject targetGameObject;
        

        protected EnemyState currentState;
        protected Rigidbody2D rb;
        [SerializeField] protected ShipResource hullHP;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            hullHP = GetComponent<ShipResource>();
            hullHP.EventResourceEmpty += Die;
            SetTarget();
        }

        protected virtual void Update()
        {

        }


        protected void SetTarget() => targetGameObject = GameObject.FindGameObjectWithTag(ReferenceManager.Singleton.SubmarineTag);
        

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

        protected void CheckDistanceToTargetFireing()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < FiringRange) currentState = EnemyState.Firing;
            else currentState = distance < chasingRange ? EnemyState.Chasing : EnemyState.Idle;
        }

        protected void CheckDistanceToTargetChasing()
        {
            float distance = Vector3.Distance(targetGameObject.transform.position, transform.position);
            if (distance < chasingRange) currentState = EnemyState.Chasing;
            else currentState = EnemyState.Idle;
        }

        private void Die() => Destroy(gameObject);

        protected void UpdateBasicRotation(Vector3 targetPosition)
        {
            //Vector till target
            Vector2 direction = targetPosition - transform.position;
            //vinkel till target
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            //rotationen som kr�vs till target som en quaternion runt z axlen
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            //Mindre del av rotationen till target (slerp)
            transform.rotation = (Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f));
        }        
    }
}

