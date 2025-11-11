using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneScript : MonoBehaviour
{
    public VideoPlayer videoPlayer; 

    void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();

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