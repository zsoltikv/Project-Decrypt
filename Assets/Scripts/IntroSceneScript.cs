using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.playOnAwake = false;

        videoPlayer.waitForFirstFrame = true;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        StartCoroutine(PlayWhenReady());
    }

    IEnumerator PlayWhenReady()
    {
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        StartCoroutine(CheckVideoEnd());
    }

    IEnumerator CheckVideoEnd()
    {
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MenuScene");
    }
}
