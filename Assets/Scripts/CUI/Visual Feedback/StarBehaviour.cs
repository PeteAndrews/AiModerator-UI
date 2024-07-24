using UnityEngine;

public class StarBehaviour : MonoBehaviour
{
    public Vector2 endPos;
    private Vector2 startPos;
    private Vector2 controlPoint;
    private RectTransform rectTransform;
    private float startTime;
    private float duration;  // Duration for the movement
    public float minDuration = 5.0f;
    public float maxDuration = 10.0f;

    public void Initialise(Vector3 newEndPos)
    {
        endPos = newEndPos;
        controlPoint = (Vector2)transform.position + new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        Debug.Log("Initializing End Position: " + endPos);
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition; // Get the initial anchored position
        startTime = Time.time;
        // Randomize the duration within the given range
        duration = Random.Range(minDuration, maxDuration);
        Debug.Log("Start Position: " + startPos + ", End Position: " + endPos + ", Duration: " + duration);
    }

    void Update()
    {
        float timeSinceStarted = Time.time - startTime;
        float fractionOfJourney = timeSinceStarted / duration; // Normalize over the random duration.

        fractionOfJourney = Mathf.SmoothStep(0.0f, 1.0f, fractionOfJourney);

        rectTransform.anchoredPosition = CalculateBezierPoint(fractionOfJourney, startPos, controlPoint, endPos);
        Debug.Log($"rectTransform.anchoredPosition:{rectTransform.anchoredPosition}");
        if (fractionOfJourney >= 1.0f)
        {
            Destroy(gameObject);
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
