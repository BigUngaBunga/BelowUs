using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeReference] private Transform followTarget;
    [SerializeField] private int zPosition;

    public void Update()
    {
        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, zPosition);
    }
}
