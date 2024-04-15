using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class GlobalTimer : MonoBehaviour
{
    public static GlobalTimer _instance { get; private set; }

    void Awake()
    {
        Debug.Log("GlobalTimer Awake() called");

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private VideoPlayer videoPlayer;

    public long CurrentFrame
    {
        get
        {
            //if (videoPlayer != null && videoPlayer.isPlaying)
            if (videoPlayer != null)
            {
                return videoPlayer.frame;
            }
            return -1; 
        }
    }

    public double Time
    {
        get
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                return videoPlayer.time;
            }
            return -1.0; 
        }
    }

    void Start()
    {
        // Replace "YourVideoPlayerGameObjectName" with the actual name of the GameObject
        GameObject videoPlayerObject = GameObject.Find("MainVideo");
        if (videoPlayerObject != null)
        {
            videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
        }
        else
        {
            Debug.LogError("VideoPlayer GameObject not found in the scene.");
        }
    }

}
