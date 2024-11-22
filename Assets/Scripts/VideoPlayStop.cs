using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayStop : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public GameObject videoObject;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        PlayStopVideo();
    }

    void PlayStopVideo()
    {
        if (videoObject.activeSelf == true)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
    }
}
