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

    public float totalPausedTime = 0f;
    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(ToggleVideoPlayPause);
        pauseButton.GetComponent<Image>().sprite = pauseSprite;
    }

    void Update()
    {
        if (isPaused)
        {
            totalPausedTime += Time.deltaTime;

            if (totalPausedTime >= 180f) // 3 minutes in seconds
            {
                pauseButton.interactable = false;
                isPaused = false; // Stop adding to the paused time
                if (!videoPlayer.isPlaying)
                {
                    videoPlayer.Play();
                    pauseButton.GetComponent<Image>().sprite = pauseSprite;
                }
            }
        }
    }

    void ToggleVideoPlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            pauseButton.GetComponent<Image>().sprite = playSprite;
            isPaused = true;
        }
        else
        {
            videoPlayer.Play();
            pauseButton.GetComponent<Image>().sprite = pauseSprite;
            isPaused = false;
        }
    }
}