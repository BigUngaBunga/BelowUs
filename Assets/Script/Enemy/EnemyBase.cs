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
    
    [SerializeField] protected float moveSpeedPatrolling;
    [SerializeField] protected float moveSpeedChasing;
    [SerializeField] protected float maxPatrolRange;

    public float MoveSpeedPatrolling { get { return moveSpeedPatrolling; } }
    public float MoveSpeedChasing { get { return moveSpeedChasing; } }
    public float MaxPatrolRange { get { return maxPatrolRange; } }

    protected float currentMoveSpeed = 0;
    [SerializeField]protected GameObject targetGameObject;
    protected List<Vector3> patrolPositions = new List<Vector3>();
    protected Vector3 currentPatrolTarget;
    protected enemyState currentState;

    [TagSelector][SerializeField] private string submarineTag;
    public string SubmarineTag { get { return submarineTag; } }

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

    private void AddRandomPatrolNumber()
    {
        patrolPositions.Add(new Vector3(Random.Range(-maxPatrolRange, maxPatrolRange), Random.Range(-maxPatrolRange, maxPatrolRange)));
    }

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
}


