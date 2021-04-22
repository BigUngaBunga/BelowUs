using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EnemyBase : NetworkBehaviour
{
    protected enum enemyState
    {
        Patrolling,
        Chasing,
        Attacking
    }

    [SerializeField] protected float moveSpeedPatrolling;
    [SerializeField] protected float moveSpeedChasing;
    [SerializeField] protected float patrolRange;
    [SerializeField] protected float chasingRange, attackingRange;
    [SerializeField] protected float health;
    [SerializeField] protected float collisionDamage;

    public float CollisionDamage
    {
        get { return collisionDamage; }
    }

    protected GameObject targetGameObject;
    protected List<Vector3> patrolPositions = new List<Vector3>();
    protected Vector3 currentPatrolTarget;    
    protected enemyState currentState;
    protected Rigidbody2D rb;



    [TagSelector][SerializeField] private string submarineTag;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }


    protected void SetTarget()
    {
        targetGameObject = GameObject.FindGameObjectWithTag(submarineTag);
    }

    private float GetRandomPatrolNumber()
    {
        return Random.Range(-patrolRange, patrolRange);
    }

    protected void CreatePatrolArea()
    {
        for (int i = 0; i < 4; i++)
        {
            patrolPositions.Add(new Vector3(GetRandomPatrolNumber(), GetRandomPatrolNumber(), 0));
        }
    }

    protected void GetNextPatrolPosition()
    {
        currentPatrolTarget = patrolPositions[0];
        patrolPositions.RemoveAt(0);
        patrolPositions.Add(new Vector3(GetRandomPatrolNumber(), GetRandomPatrolNumber(), 0));
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


