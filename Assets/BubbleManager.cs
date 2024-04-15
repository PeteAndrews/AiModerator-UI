using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BubbleManager : MonoBehaviour
{
    private FrameJsonLoader loader = new FrameJsonLoader();
    private int gameTime;

    public List<int> bubbleEventTimes;
    public List<int> bubbleEventPersons;
    
    public Camera worldCamera;
    
    private int _bubbleEventIndex;
    private HashSet<int> _generatedTimes;
    private Vector2 _frameTie;
    
    public GameObject bubblePrefab;
    public RectTransform canvasTransform;
    //public VideoPlayer videoPlayer;

    public RectTransform startPosition;
    public Color bubbleColor;
    public string text;
    
    public RectTransform endPosition;
    public RectTransform barPosition;

    void Start()
    {
        _bubbleEventIndex = 0;
        gameTime = (int)Time.realtimeSinceStartup;
        _generatedTimes = new HashSet<int>();
    }


    // Update is called once per frame
    void Update()
    {
        gameTime = (int)Time.realtimeSinceStartup;
        
        //Debug.Log(gameTime);
        
        if(bubbleEventTimes.Contains(gameTime) && !_generatedTimes.Contains(gameTime))
        {
            AddBubble(gameTime, _bubbleEventIndex);
            _generatedTimes.Add(gameTime); //Mark this point in time when a Bubble has been created
            _bubbleEventIndex++;
            // AddBubbleToWorldSpace();
        }
        
    }
    
    
    void AddBubble(int time, int eventIndex)
    {
        int frameNumber = time * 25;
        //Debug.Log(frameNumber);
        FrameData framedata = loader.LoadFrameData(frameNumber);
        //Debug.Log(framedata.track_ids[0]);
        
        GameObject bubble = Instantiate(bubblePrefab, canvasTransform);
        Bubble bubbleScript = bubble.GetComponent<Bubble>();
        bubbleScript.startPosition = startPosition;
        
        //Person Tag, Position of the bubble
        if (bubbleEventPersons[eventIndex] == 1)
        {
            Debug.Log("Trump");
            _frameTie = new Vector2(framedata.boxes[2][2], (-1)*framedata.boxes[2][1]);
            Debug.Log(_frameTie);
            bubbleScript.bubbleColor = Color.blue;
        }
        else if (bubbleEventPersons[eventIndex] == 2)
        {
            Debug.Log("Biden");
            _frameTie = new Vector2(framedata.boxes[3][2], (-1)*framedata.boxes[3][1]);
            Debug.Log(_frameTie);
            bubbleScript.bubbleColor = Color.red;
        }
        
        bubbleScript.startPosition.anchorMin = new Vector2(0, 1);
        bubbleScript.startPosition.anchorMax = new Vector2(0, 1);
        bubbleScript.startPosition.anchoredPosition = _frameTie + new Vector2(240, 50);
        
        
        
        bubbleScript.text = text;
        bubbleScript.lifeTime = 10;
        bubbleScript.endPosition = endPosition;
        bubbleScript.barPosition = barPosition;
    }
    
}
