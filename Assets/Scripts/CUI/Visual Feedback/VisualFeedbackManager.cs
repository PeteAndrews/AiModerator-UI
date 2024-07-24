using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ReactButtonPair
{
    public string id;
    public GameObject reactObject;
}

public class VisualFeedbackManager : MonoBehaviour
{
    public GameObject starPrefab;
    public FrameJsonLoader frameJsonLoader;
    public Canvas canvas;
    private bool isSubscribed = false;
    [SerializeField] private List<ReactButtonPair> reactButtonList = new List<ReactButtonPair>();
    private Dictionary<string, GameObject> reactButtonDict;

    private void OnEnable()
    {
        if (CuiManager.Instance != null)
        {
            CuiManager.Instance.InstantiatePollFeedback += OnPollFeedback;
            CuiManager.Instance.InstantiateReactFeedback += OnReactFeedback;
        }
    }
    private void OnDisable()
    {
        CuiManager.Instance.InstantiatePollFeedback -= OnPollFeedback;
        CuiManager.Instance.InstantiateReactFeedback -= OnReactFeedback;

    }
    private void Awake()
    {
        reactButtonDict = reactButtonList.ToDictionary(pair => pair.id, pair => pair.reactObject);
    }
    private void Start()
    {
        // Ensures subscription if OnEnable missed the initialization
        if (CuiManager.Instance != null && !isSubscribed)
        {
            CuiManager.Instance.InstantiatePollFeedback += OnPollFeedback;
            CuiManager.Instance.InstantiateReactFeedback += OnReactFeedback;

            isSubscribed = true;
        }
    }
    private void OnReactFeedback(string id, string name, Transform transform)
    {
        GameObject reactIcon;
        reactButtonDict.TryGetValue(id, out reactIcon);
        if (reactIcon != null)
        {
            ReactFeedback(reactIcon, transform, name);
        }
    }
    /*
    private void ReactFeedback(GameObject reactIcon, Transform startPos, string candidate)
    {
        FrameData frameData = frameJsonLoader.LoadFrameData((int)GlobalTimer._instance.CurrentFrame);
        if (frameData == null) return;

        string name = candidate.ToLower() + "_tie";
        int matchingIndex = Array.IndexOf(frameData.track_ids, frameData.track_ids.FirstOrDefault(id => id.Contains(name)));
        Vector3 endPosition = matchingIndex != -1 ? FormatEndPosition(frameData, matchingIndex) : Vector3.zero;
        Vector3 canvasPos = matchingIndex != -1 ? ConvertScreenCoordinatesToCanvas(new Vector2(endPosition.x, endPosition.y), canvas) : Vector3.zero;

        // Instantiate at start position
        GameObject icon = Instantiate(reactIcon, startPos.position, Quaternion.identity, canvas.transform);
        EmojiBehaviour emojiBehaviour = icon.GetComponent<EmojiBehaviour>();
        if (emojiBehaviour != null)
        {
            emojiBehaviour.Initialise(new Vector3(canvasPos.x, canvasPos.y, 0));
        }
    }*/
    private void ReactFeedback(GameObject reactIcon, Transform startPos, string candidate)
    {
        FrameData frameData = frameJsonLoader.LoadFrameData((int)GlobalTimer._instance.CurrentFrame);
        if (frameData == null) return;

        // Attempt to find a matching index for the primary and alternative candidate names
        string primaryName = candidate.ToLower() + "_tie";
        string alternativeName = candidate.ToLower();
        int matchingIndex = Array.IndexOf(frameData.track_ids, frameData.track_ids.FirstOrDefault(id => id.Contains(primaryName)));
        Vector3 endPosition = Vector3.zero;

        if (matchingIndex == -1)
        {
            matchingIndex = Array.IndexOf(frameData.track_ids, frameData.track_ids.FirstOrDefault(id => id.Contains(alternativeName)));

            if(matchingIndex != -1) 
            {
                int[] box = frameData.boxes[matchingIndex];
                endPosition = new Vector3((box[0] + box[2]) / 2f, (box[1] + box[3]) / 2f, 0f);
            }
            else
            {
                endPosition = new Vector3(1920/2f, 1080/2f, 0f);
            }
        }
        else
        {
            endPosition = FormatEndPosition(frameData, matchingIndex);

        }
        // Convert to Canvas coordinates and instantiate the icon
        Vector3 canvasPos = ConvertScreenCoordinatesToCanvas(new Vector2(endPosition.x, endPosition.y), canvas);
        GameObject icon = Instantiate(reactIcon, startPos.position, Quaternion.identity, canvas.transform);
        EmojiBehaviour emojiBehaviour = icon.GetComponent<EmojiBehaviour>();
        if (emojiBehaviour != null)
        {
            emojiBehaviour.Initialise(new Vector3(canvasPos.x, canvasPos.y, 0));
        }
    }

    private Vector3 FormatEndPosition(FrameData frameData, int matchingIndex)
    {
        int[] box = frameData.boxes[matchingIndex];
        Vector3 endPosition = new Vector3(
            (box[0] + box[2]) / 2f,  // Middle of the left and right x-coordinates
            box[1],                  // Top y-coordinate assuming top-left origin
            0f                       // z-coordinate, typically 0 for UI elements
        );
        return endPosition;
    }
    private void OnPollFeedback(string candidate, Transform starPos)
    {
        FrameData frameData = frameJsonLoader.LoadFrameData((int)GlobalTimer._instance.CurrentFrame);
        if (frameData != null)
        {
            string name = candidate.ToLower() + "_tie";
            int matchingIndex = Array.IndexOf(frameData.track_ids, frameData.track_ids.FirstOrDefault(id => id.Contains(name)));
            if (matchingIndex != -1)
            {
                //Format the position
                Vector3 endPosition = FormatEndPosition(frameData, matchingIndex);
                // Convert to Canvas coordinates directly here
                Vector3 canvasPos = ConvertScreenCoordinatesToCanvas(new Vector2(endPosition.x, endPosition.y), canvas);
                GameObject star2 = Instantiate(starPrefab, starPos.position, Quaternion.identity, canvas.transform); // Instantiate at start position
                // Initialize the StarBehaviour script
                EmojiBehaviour starBehaviour = star2.GetComponent<EmojiBehaviour>();
                if (starBehaviour != null)
                {
                    starBehaviour.Initialise(new Vector3(canvasPos.x, canvasPos.y, 0));
                }
            }
        }
    }
    private Vector3 ConvertScreenCoordinatesToCanvas(Vector2 screenCoordinates, Canvas canvas)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

        // Calculate the scaling factors based on the canvas size and the reference resolution.
        float scaleFactorX = canvasRect.rect.width / canvasScaler.referenceResolution.x;
        float scaleFactorY = canvasRect.rect.height / canvasScaler.referenceResolution.y;
        // Adjust screen coordinates to center origin (0,0) and apply the scale factor.
        Vector2 centeredCoordinates = new Vector2(
            (screenCoordinates.x - 960) * scaleFactorX, // Adjust x to center
            (540 - screenCoordinates.y) * scaleFactorY  // Adjust y to center and flip
        );
        Vector2 finalPosition = new Vector2(
            centeredCoordinates.x + (canvasRect.pivot.x * canvasRect.rect.width) - canvasRect.rect.width / 2,
            centeredCoordinates.y + (canvasRect.pivot.y * canvasRect.rect.height) - canvasRect.rect.height / 2
        );

        return new Vector3(finalPosition.x, finalPosition.y, 0);
    }
}
