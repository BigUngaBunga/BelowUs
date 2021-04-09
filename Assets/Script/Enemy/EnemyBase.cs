using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected enum enemyState
    {
        Patrolling,
        Chasing,
        Attacking
    }

    protected string targetName;
    public float moveSpeedPatrolling;
    public float moveSpeedChasing;
    protected float currentMoveSpeed = 0;
    [SerializeField]protected GameObject targetGameObject;
    protected List<Vector3> patrolPositions = new List<Vector3>();
    protected Vector3 currentPatrolTarget;
    private float patrolRange = 5;
    protected enemyState currentState;

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
}


