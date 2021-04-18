using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookScript : EnemyBase
{
   
    private float maxSpeed = 0.1f;
    [SerializeField] float movementSpeed = 5f;

    protected override void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        moveSpeedPatrolling = 5f;
        moveSpeedChasing = 7f;
        currentState = enemyState.Patrolling;
        CreatePatrolArea();
        SetTarget();
        GetNextPatrolPosition();
    }

    protected override void Update()
    {
        
    }
}
