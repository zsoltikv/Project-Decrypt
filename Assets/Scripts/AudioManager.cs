using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Clips")]
    public List<AudioClip> musicClips; 

    [Header("Settings")]
    public float volume = 0.5f;

    private AudioSource audioSource;
    private AudioClip lastClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = volume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene" || scene.name == "GameScene")
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else
        {
            if (!audioSource.isPlaying)
                PlayNext();
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != "MenuScene" && scene.name != "GameScene")
            {
                PlayNext();
            }
        }
    }

    private void PlayNext()
    {
        if (musicClips.Count == 0) return;

        AudioClip nextClip;
        do
        {
            int index = Random.Range(0, musicClips.Count);
            nextClip = musicClips[index];
        } while (nextClip == lastClip && musicClips.Count > 1);

        lastClip = nextClip;
        audioSource.clip = nextClip;
        audioSource.loop = true; 
        audioSource.Play();
    }
}