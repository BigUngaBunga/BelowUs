using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine_Movement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private float subSpeed, subSpeedChange;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        subSpeed = 1;
        subSpeedChange = 0.0005f;
    }
    
    void Update()
    {
        if (transform.rotation.eulerAngles.y == 180)
        {
            if (transform.rotation.eulerAngles.z <= 90 || transform.rotation.eulerAngles.z >= 100)
            {
                if (Input.GetButton("ReverseRight"))
                {
                    transform.Rotate(0, 0, 0.1f);
                }
            }
            if (transform.rotation.eulerAngles.z <= 100 || transform.rotation.eulerAngles.z >= 270)
            {
                if (Input.GetButton("ReverseLeft"))
                {
                    transform.Rotate(0, 0, -0.1f);
                }
            }
        }
        else
        {
            if (transform.rotation.eulerAngles.z <= 90 || transform.rotation.eulerAngles.z >= 100)
            {
                if (Input.GetButton("RotateLeft"))
                {
                    transform.Rotate(0, 0, 0.1f);
                }
            }
            if (transform.rotation.eulerAngles.z <= 100 || transform.rotation.eulerAngles.z >= 270)
            {
                if (Input.GetButton("RotateRight"))
                {
                    transform.Rotate(0, 0, -0.1f);
                }
            }
        }
        if (Input.GetButton("MoveForward"))
        {
            rb2D.velocity = -transform.right * subSpeed; 
        }
        if (Input.GetButton("MoveBackwards"))
        {
            rb2D.velocity = transform.right * subSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Quaternion rotation = transform.rotation;
            transform.localRotation = new Quaternion(rotation.x * -1, rotation.y, rotation.z, rotation.w * -1);
            transform.Rotate(0, 180, 0);
        }
        if(!Input.GetButton("MoveBackwards") && !Input.GetButton("MoveForward"))
        {
            rb2D.velocity = new Vector2(0, 0);
        }
        if (Input.GetButton("IncreaseSpeed") && subSpeed <= 3)
        {
            subSpeed += subSpeedChange;
        }
        if (Input.GetButton("DecreaseSpeed") && subSpeed >= 1)
        {
            subSpeed -= subSpeedChange;
        }
    }
}
