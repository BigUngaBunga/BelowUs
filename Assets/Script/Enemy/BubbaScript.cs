using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbaScript : EnemyBase
{
    public Transform TargetLocation;
    private Rigidbody2D rb;
    private Vector3 direction;

    protected override void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        moveSpeedPatrolling = 5f;
        moveSpeedChasing = 7f;
        targetName = "GameObject";
        currentState = enemyState.Patrolling;
        CreatePatrolArea();
        SetTarget();
        GetNextPatrolPosition();
    }

    protected override void Update()
    {
        switch (currentState)
        {
            case enemyState.Patrolling:
                UpdateRotationPatrolling();
                UpdateMovementPatrolling();
                if (Vector3.Distance(targetGameObject.transform.position, transform.position) < 50f)
                {
                    currentState = enemyState.Chasing;
                }

                break;
            case enemyState.Chasing:
                UpdateRotationChasing();
                UpdateMovementChasing();
                break;
        }
    }

    #region chasing

    protected void UpdateRotationChasing()
    {
        direction = targetGameObject.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        rb.rotation = -angle;
        direction.Normalize();
    }
    protected void UpdateMovementChasing()
    {
        rb.MovePosition((Vector3)transform.position + (direction * moveSpeedChasing * Time.deltaTime));
    }
    #endregion chasing

    #region patrolling
    protected void UpdateRotationPatrolling()
    {
        //direction = currentPatrolTarget - transform.position;
        //float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        ////rb.rotation = ;

        //Vector3 eulerAngleVelocity = new Vector3(0, 0, angle);
        //Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * 2f);
        //rb.MoveRotation(rb.rotation * deltaRotation);

        //var rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(direction), 20f * Time.deltaTime);
        //rb.MoveRotation(rb.rotation * rotation);


        
    }

    protected void UpdateMovementPatrolling()
    {
        rb.MovePosition((Vector3)transform.position + (direction * moveSpeedPatrolling * Time.deltaTime));

        if(Vector3.Distance(currentPatrolTarget, transform.position) < 1f)
        {
            base.GetNextPatrolPosition();
        }
    }

    #endregion patrolling
}
