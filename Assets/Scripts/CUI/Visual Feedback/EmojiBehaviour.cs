using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour
{
    // Movement parameters
    public Vector2 endPos;
    public float minDuration = 5.0f;
    public float maxDuration = 10.0f;
    private Vector2 startPos;
    private Vector2 controlPoint;
    private float startTime;
    private float duration;

    // Spin parameters
    public float maxSpeed = 360.0f;
    public float minSpeed = 50.0f;
    public float acceleration = 50.0f;
    public float deceleration = 25.0f;
    private float currentSpeed;
    private bool isAccelerating = true;

    // Rotation reset and grow
    private bool isGrowing = false;
    private float growStartTime;
    public float growDuration = 3.0f;
    public float growFactor = 1.5f;

    private RectTransform rectTransform;


    public void Initialise(Vector3 newEndPos)
    {
        endPos = newEndPos;
        controlPoint = (Vector2)transform.position + new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        startTime = Time.time;
        duration = Random.Range(minDuration, maxDuration);
        controlPoint = (Vector2)transform.position + new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        currentSpeed = minSpeed;
    }

    void Update()
    {
        if (!isGrowing)
        {
            UpdateMovementAndSpin();
        }
        else
        {
            if (GrowAndResetRotation())
            {
                Destroy(gameObject); 
                TabManager.Instance.OnReactDisable();
            }
        }
    }

    void UpdateMovementAndSpin()
    {
        float timeSinceStarted = Time.time - startTime;
        float fractionOfJourney = timeSinceStarted / duration;
        fractionOfJourney = Mathf.SmoothStep(0.0f, 1.0f, fractionOfJourney);
        rectTransform.anchoredPosition = CalculateBezierPoint(fractionOfJourney, startPos, controlPoint, endPos);

        // Manage spin
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

        if (fractionOfJourney >= 1.0f)
        {
            isGrowing = true;
            growStartTime = Time.time;
            rectTransform.localRotation = Quaternion.identity;  // Reset rotation
        }
    }

    bool GrowAndResetRotation()
    {
        float growTime = Time.time - growStartTime;
        float fractionOfGrow = growTime / growDuration;
        if (fractionOfGrow < 1.0f)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * growFactor, fractionOfGrow);
            return false;
        }
        else
        {
            return true;  // Indicates growth is complete and the GameObject can be destroyed
        }
    }

    Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}
