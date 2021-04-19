using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private Vector3 mousePos, pos;
    private float angleRad, angleDeg, offset, subRotation;

    void Start()
    {
        offset = 90;
    }
    
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos = this.transform.position;

        subRotation = (float)((Mathf.Atan2(pos.y - transform.parent.position.y, pos.x - transform.parent.position.x) / Math.PI) * 180) + 7;

        float angleRad = Mathf.Atan2(mousePos.y - pos.y, mousePos.x - pos.x);

        float angleDeg = (float)((angleRad / Math.PI) * 180);
        if (angleDeg < 0)
            angleDeg += 360;
        

        if(subRotation < 0)
        {
            subRotation += 360;
        }

        if (angleDeg +7 <= 54 + subRotation && angleDeg + 7 >= -54 + subRotation)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, angleDeg + 90);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //pos = new Vector3(pos.x, pos.y, -1);
        }
    }
}
