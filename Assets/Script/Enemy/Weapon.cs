using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject bulletPrefab;   

    public void shoot()
    {
        Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
    }
}
