using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSpin : MonoBehaviour
{
    public float maxSpeed = 360.0f; // Maximum speed in degrees per second
    public float minSpeed = 50.0f; // Minimum speed to not go below
    public float acceleration = 50.0f; // Acceleration rate until reaching max speed
    public float deceleration = 25.0f; // Deceleration rate until reaching min speed
    private float currentSpeed;
    private bool isAccelerating = true; // Control flag for acceleration or deceleration

    private void OnEnable()
    {
        currentSpeed = minSpeed;
    }

    void Update()
    {
        if (isAccelerating)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
                isAccelerating = false; 
            }
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed <= minSpeed)
            {
                currentSpeed = minSpeed;
                isAccelerating = true; 
            }
        }
        transform.Rotate(0, 0, -currentSpeed * Time.deltaTime);
    }
}
