using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FirstCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.playOnAwake = false;

        // Feliratkozunk a videó végére
        videoPlayer.loopPointReached += OnVideoFinished;

        // Videó lejátszása
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video ended! Loading GameScene...");
        SceneManager.LoadScene("GameScene");
    }
}