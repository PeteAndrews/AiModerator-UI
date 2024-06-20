using Unity.Burst.CompilerServices;
using UnityEngine;

public class UserController : MonoBehaviour
{
    public static UserController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool isSwipe = false;
    public float minimumSwipeDistance = 50f; // Pixels
    private int tapCount = 0;
    private float lastTapTime = 0;
    private float doubleTapTime = 0.3f; // Maximum time interval between taps

    // Variables for pinch detection
    private float initialPinchDistance;
    private Vector2 initialPinchCenter;
    private bool isPinching = false;

    public delegate void SwipeEventHandler(string direction);
    public event SwipeEventHandler OnSwipeEvent;
    public delegate void DoubleTapEventHandler();
    public event DoubleTapEventHandler OnDoubleTapEvent;
    public delegate void SingleTapEventHandler();
    public event SingleTapEventHandler OnSingleTapEvent;
    public delegate void PinchZoomEventHandler(float factor);
    public event PinchZoomEventHandler OnPinchZoomEvent;

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        // Handle Touch Input for Deployment
        HandleTouchInput();
#endif
    }

#if UNITY_EDITOR
    private float simulatedPinchDistance = 100f;
    private float zoomSpeed = 0.1f;
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessInput(Input.mousePosition, TouchPhase.Began);
        }
        else if (Input.GetMouseButton(0))
        {
            ProcessInput(Input.mousePosition, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ProcessInput(Input.mousePosition, TouchPhase.Ended);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            HandleSimulatedPinch(scroll);
        }

    }
    private void HandleSimulatedPinch(float scrollDelta)
    {
        simulatedPinchDistance += scrollDelta * 100; // Adjust scaling to simulate pinch distance changes
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (simulatedPinchDistance < 10) simulatedPinchDistance = 10; // Prevent it from going to zero or negative
        float zoomFactor = simulatedPinchDistance / 100; // Normalize the distance to a zoom factor

        HandlePinchZoom(mousePos, zoomFactor);
    }
#endif
    private Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
        return new Vector2(worldPoint.x, worldPoint.y);
    }

    private void ProcessInput(Vector2 position, TouchPhase phase)
    {
        Vector2 touchPosWorld = ScreenToWorld(position);
        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Vector2.zero);

        switch (phase)
        {
            case TouchPhase.Began:
                HandleTouchBegan(position, hitInformation);
                break;
            case TouchPhase.Moved:
                HandleTouchMove(position, hitInformation);
                break;
            case TouchPhase.Ended:
                HandleTouchEnded(position, hitInformation);
                break;
        }
    }
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            ProcessInput(touch.position, touch.phase);
        }
        else if (Input.touchCount > 1)
        {
            HandlePinch();
        }
    }
    private void HandlePinch()
    {
        // Get two touches and calculate the pinch distance and pinch center
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (!isPinching)
        {
            initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            initialPinchCenter = (touch0.position + touch1.position) / 2;
            isPinching = true;
        }
        else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
        {
            var currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            var factor = currentPinchDistance / initialPinchDistance;
            HandlePinchZoom(initialPinchCenter, factor);
            initialPinchDistance = currentPinchDistance;
        }
        else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
        {
            isPinching = false;
        }
    }
    private void HandleTouchBegan(Vector2 position, RaycastHit2D hitInformation)
    {
        touchStart = position;
        isSwipe = false;
        if (Time.time - lastTapTime < doubleTapTime && Vector2.Distance(position, touchStart) < 10f)
        {
            tapCount++;
        }
        else
        {
            tapCount = 1;
        }
        lastTapTime = Time.time;

        // If double tap is detected, handle it
        if (tapCount == 2)
        {
            HandleDoubleTap(position, hitInformation);
            tapCount = 0; // Reset tap count after handling double tap
        }
    }
    private void HandleTouchMove(Vector2 position, RaycastHit2D hitInformation)
    {
        touchEnd = position;
        if (!isSwipe && Vector2.Distance(touchStart, touchEnd) > minimumSwipeDistance)
        {
            isSwipe = true;
            if (hitInformation.collider != null)
            {
                HandleSwipe(touchStart, touchEnd, hitInformation.collider.gameObject.name);
            }
            else
            {
                HandleSwipe(touchStart, touchEnd, null);
            }
        }
    }
    private void HandleTouchEnded(Vector2 position, RaycastHit2D hitInformation)
    {
        touchEnd = position;
        if (!isSwipe)
        {
            HandleTap(touchEnd, hitInformation);
        }
    }
    private void HandlePinchZoom(Vector2 center, float factor)
    {
        Debug.Log("Pinch Zoom at: " + center + " with scale factor: " + factor);
        OnPinchZoomEvent?.Invoke(factor);
    }
    private void HandleSwipe(Vector2 start, Vector2 end, string hitGameObjectName)
    {
        Vector2 direction = end - start;
        if (hitGameObjectName != null)
        {
            HandleSwipeDirection(direction);
        }
    }
    private void HandleSwipeDirection(Vector2 direction)
    {
        // Change the comparison to focus on the y-component
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0)
            {
                Debug.Log("Swipe Up Detected");
                OnSwipeEvent?.Invoke("up");
            }
            else
            {
                Debug.Log("Swipe Down Detected");
                OnSwipeEvent?.Invoke("down");
            }
        }
    }
    private void HandleTap(Vector2 position, RaycastHit2D hitInformation)
    {
        Debug.Log("Tap Detected at: " + position);
        if (hitInformation.collider != null)
        {
            if (hitInformation.collider.CompareTag("Button"))
            {
                // Invoke event or handle button tap
                Debug.Log("Button was tapped.");
            }
            else
            {
                Debug.Log("Object tapped was not a button.");
            }
        }
        else
        {
            // Handle tap when no objects were hit
            OnSingleTapEvent?.Invoke();
        }
    }
    private void HandleDoubleTap(Vector2 position, RaycastHit2D hitInformation)
    {
        if (hitInformation.collider != null)
        {
            Debug.Log("Double Tap Detected at: " + position + " on GameObject: " + hitInformation.collider.gameObject.name);
            OnDoubleTapEvent?.Invoke();
        }
    }
    public void SetTouchInteractionEnabled(bool enabled)
    {
        this.enabled = enabled; // Enable or disable this UserController component
    }
}