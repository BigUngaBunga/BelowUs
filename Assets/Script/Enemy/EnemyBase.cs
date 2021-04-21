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
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float patrolRange = 5;
    [SerializeField] protected float chasingRange, attackingRange;
    [SerializeField] public float health;

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
}


