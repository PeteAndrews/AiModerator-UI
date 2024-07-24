using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button pauseButton;
    public Sprite pauseSprite;   
    public Sprite playSprite;   

    void Start()
    {
        pauseButton.onClick.AddListener(ToggleVideoPlayPause);
        pauseButton.GetComponent<Image>().sprite = pauseSprite;
    }

    void ToggleVideoPlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            pauseButton.GetComponent<Image>().sprite = playSprite; 
        }
        else
        {
            videoPlayer.Play();
            pauseButton.GetComponent<Image>().sprite = pauseSprite; 
        }
    }
}
