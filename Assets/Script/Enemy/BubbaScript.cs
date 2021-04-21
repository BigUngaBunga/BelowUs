using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BubbaScript : EnemyBase
{
    protected override void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
       
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
                CheckForFlip();                
                break;
            case enemyState.Chasing:
                UpdateRotationChasing();
                UpdateMovementChasing();
                CheckForFlip();
                break;
            case enemyState.Attacking:
                UpdateRotationChasing();
                UpdateMovementChasing();
                CheckForFlip();
                break;
        }
    }

    #region chasing

    
    protected void UpdateMovementChasing()
    {
        Vector2 direction = (targetGameObject.transform.position - transform.position).normalized;
        Vector2 movement = direction * movementSpeed * Time.deltaTime;
        rb.AddForce(movement);
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
        rb.MovePosition((Vector3)transform.position + (direction * moveSpeedPatrolling * Time.deltaTime));

        if (Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
        {
            base.GetNextPatrolPosition();
        }
    }    

    #endregion patrolling   
}
