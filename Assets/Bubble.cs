using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class Bubble : MonoBehaviour
{
    public float lifeTime;
    public RectTransform startPosition; 
    public Color bubbleColor; 
    public string text;
    
    public RectTransform endPosition;
    public RectTransform barPosition;
    private float startTime;
    private bool isSaved = false;
    
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    
    
    void Start()
    {
        //set the start position of the bubble
        
        GetComponent<RectTransform>().anchorMin = startPosition.anchorMin;
        GetComponent<RectTransform>().anchorMax = startPosition.anchorMax;
        GetComponent<RectTransform>().anchoredPosition = startPosition.anchoredPosition;
        GetComponent<Image>().color = bubbleColor;
        
        startTime = Time.time;
        lifeTime = 20;
    }
    
    void Update()
    {
        //transform.position = transform.position + new Vector3(0, 0.1f, 0);
        MoveToEndPosition();
        
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }
    
    void MoveToEndPosition()
    {
        // slowly move the bubble to the end position
        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, endPosition.anchoredPosition, 0.001f);
    }
    
    
    //Destroy the bubble after a certain amount of time
    void LateUpdate()
    {
        if (Time.time - startTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }
    
    //Save the bubble in the UI menu for later interaction
    private void SaveBubble()
    {
        if (!isSaved)
        {
            isSaved = true;
            //save it into UI menu
            //Debug.Log("Bubble saved");
        }
    }
    
    
    //Click on the bubble to expand the bubble and display the text
    private void OnClick()
    {
        
    }
    
    //Swipe to destroy the bubble
    private void DetectSwipe()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, endPosition.anchoredPosition, 0.001f);
    }
    
}