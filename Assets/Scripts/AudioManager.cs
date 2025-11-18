using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Clips")]
    public AudioClip introSong;
    public List<AudioClip> shuffleSongs;

    [Header("Settings")]
    public float volume = 0.5f;

    private AudioSource audioSource;
    private AudioClip lastShuffleClip;

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
        audioSource.volume = volume;
        audioSource.loop = false; // alapból ne loopoljon
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ha épp a FirstCutsceneScene-ben vagyunk, ne indítsunk zenét
        if (scene.name == "FirstCutsceneScene")
            return;

        if (scene.name == "MenuScene")
        {
            if (!audioSource.isPlaying)
                PlayIntro();
        }
        else if (scene.name == "GameScene")
        {
            if (audioSource.isPlaying && audioSource.clip == introSong)
                audioSource.Stop();

            if (!audioSource.isPlaying)
                PlayNextShuffle();
        }
        else
        {
            if (!audioSource.isPlaying)
                PlayNextShuffle();
        }
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        // Ha FirstCutsceneScene-en vagyunk, ne csináljon semmit
        if (scene.name == "FirstCutsceneScene")
            return;

        if (!audioSource.isPlaying)
        {
            if (scene.name == "MenuScene")
                PlayIntro();
            else
                PlayNextShuffle();
        }
    }

    private void PlayIntro()
    {
        if (audioSource.clip != introSong)
        {
            audioSource.clip = introSong;
            audioSource.loop = true; // intro mindig loop
            audioSource.Play();
        }
    }

    private void PlayNextShuffle()
    {
        if (shuffleSongs.Count == 0) return;

        AudioClip nextClip;
        do
        {
            int index = Random.Range(0, shuffleSongs.Count);
            nextClip = shuffleSongs[index];
        } while (nextClip == lastShuffleClip && shuffleSongs.Count > 1);

        lastShuffleClip = nextClip;
        audioSource.clip = nextClip;
        audioSource.loop = false; // egy klip mindig végigmegy, nem loop
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

}