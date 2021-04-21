using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TempFiriingscript : MonoBehaviour
{

    private float timeElapsed = 0;
    private Weapon weapon;
    private Transform firePoint;

    [SerializeField] public GameObject targetGameObject;
    [SerializeField] private float timeBetweenShoots = 2;
   
    void Start()
    {
        weapon = this.GetComponent<Weapon>();
        firePoint = gameObject.transform.Find("FirePoint");
    }

    [Server]
    void Update()
    {
        UpdateFirePointRotation();
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeBetweenShoots)
        {
            timeElapsed -= timeBetweenShoots;
            weapon.shoot();
        }
    }

    private void UpdateFirePointRotation()
    {
        Vector2 direction = targetGameObject.transform.position - firePoint.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        firePoint.rotation = (Quaternion.Slerp(firePoint.transform.rotation, rotation, Time.deltaTime * 5f));
    }
}
