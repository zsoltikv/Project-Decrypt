using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FirstCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.playOnAwake = false;

        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("GameScene");
    }
}