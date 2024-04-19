using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    PlayerUI playerUI;

    // Start is called before the first frame update
    void Start()
    {
        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();

        videoPlayer.prepareCompleted += CheckIsVideoStarting;
        videoPlayer.loopPointReached += CheckIsVideoOver;
    }

    void CheckIsVideoStarting(UnityEngine.Video.VideoPlayer vp)
    {
        playerUI.loadingUi.SetActive(false);
    }

    void CheckIsVideoOver(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene("Menu");
    }
}
