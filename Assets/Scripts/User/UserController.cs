using TMPro;
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
    public float pinchSensitivity = 800f; // Sensitivity of the pinch gesture   /500?

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
    private float tapCooldown = 0.2f; // 200 milliseconds
    public TextMeshProUGUI textPinchFactor;
    private float clampMin = 0.5f;
    private float clampMax = 2.0f;
    private void Start()
    {
        SetTouchInteractionEnabled(false);
        lastTapTime = -tapCooldown; // Ensures the first tap is processed.

    }

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
        simulatedPinchDistance += scrollDelta * 500;

        simulatedPinchDistance = Mathf.Clamp(simulatedPinchDistance, 50, 200);

        //float zoomFactor = (simulatedPinchDistance - 50) / (200 - 50);
        float zoomFactor = (simulatedPinchDistance - 50) / 100;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        //RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Vector2.zero);
        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld, Vector2.down, 10f);

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

    /*
private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            if (isPinching)
            {
                Touch touch = Input.GetTouch(0);
                ProcessInput(touch.position, touch.phase);
            }

        }
        else if (Input.touchCount > 1)
        {
            HandlePinch();
            isSwipe = false;
        }
    }*/
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // Allow single touch inputs to be processed normally if not pinching
            if (!isPinching)
            {
                Touch touch = Input.GetTouch(0);
                ProcessInput(touch.position, touch.phase);
            }
        }
        else if (Input.touchCount > 1)
        {
            HandlePinch();
            isSwipe = false; // Prevent swipe detection during pinch
        }
        else
        {
            // Reset flags when no touches or unexpected touch patterns
            isPinching = false;
            isSwipe = false;
        }
    }
    private void HandlePinch()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (!isPinching)
        {
            initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            isPinching = true;
        }
        else if ((touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved) && isPinching)
        {
            var currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            var pinchChange = currentPinchDistance - initialPinchDistance;
            var factor = 1 + pinchChange / pinchSensitivity;
            factor = Mathf.Clamp(factor, clampMin, clampMax);

            HandlePinchZoom(initialPinchCenter, factor);
            // Optionally update initialPinchDistance here only under certain conditions
        }
        else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
        {
            isPinching = false;
            isSwipe = false;

        }

    }
    /*
    private void HandlePinch()
    {
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
            var pinchChange = currentPinchDistance - initialPinchDistance;
            var factor = 1 + pinchChange / (pinchSensitivity);
            factor = Mathf.Clamp(factor, NetworkSettings.Instance.clampMin, NetworkSettings.Instance.clampMax); 

            HandlePinchZoom(initialPinchCenter, factor);
            initialPinchDistance = currentPinchDistance;
        }
        else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
        {
            isPinching = false;
        }
    }*/
    private void HandlePinchZoom(Vector2 center, float factor)
    {
        textPinchFactor.text = factor.ToString();
        Debug.Log("Pinch Zoom at: " + center + " with scale factor: " + factor);
        OnPinchZoomEvent?.Invoke(factor);
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

        if (tapCount == 2)
        {
            HandleDoubleTap(position, hitInformation);
            tapCount = 0; 
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
            string gameObjectName = hitInformation.collider.gameObject.name;
            string gameObjectTag = hitInformation.collider.gameObject.tag;

            Debug.Log($"Tap processed on GameObject: {gameObjectName}, Tag: {gameObjectTag}");

            if (hitInformation.collider.CompareTag("FunctionButton"))
            {
                Debug.Log("Button was tapped.");
            }
            else
            {
                Debug.Log("Object tapped was not a button.");
            }
        }
        else
        {
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
        this.enabled = enabled;
    }
}