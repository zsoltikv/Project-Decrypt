/* ----- EXTRA FUNKCIÓK: -----
 * 
 *  - egyedi Cutscene rendszer (Intro, bevezetõ és Outro)
 *  - Achievement rendszer (26 db elõre definiált achievement)
 *  - Leaderboard rendszer (lokális mentéssel, 4 adattaggal) 
 *  - Difficulty rendszer (befolyásolja a minigame-k nehézségét)
 *  - Véletlenszerû jelszó lehetõsége
 *  - Több fajta minigame eltérõ típusokkal (idõkorlátos, logikai, memória, ügyességi)
 *  - Mátrix ihletettségû dizájn
 *  - Haptic feedback rendszer
 *  - Hajszálvékony történet
 *  - UI elemek animációja
 *  - Laptop UI dátum és idõ kijelzése (szándékosan 2013 a történet végett)
 *  
 *  - GitHub repository linkje: https://github.com/zsoltikv/Project-Decrypt
 */

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

    public bool musicDisabled = false;

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
        audioSource.loop = false;

        if (!musicDisabled)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "MenuScene")
        {
            musicDisabled = false;
        }

        if (musicDisabled)
        {
            audioSource.Stop();
            return;
        }

        if (scene.name == "FirstCutsceneScene")
            return;

        if (scene.name == "MenuScene")
        {
            if (!audioSource.isPlaying)
                PlayIntro();
        }
        else
        {
            if (!audioSource.isPlaying)
                PlayNextShuffle();
        }
    }

    private void Update()
    {
        if (musicDisabled)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            return;
        }

        Scene scene = SceneManager.GetActiveScene();

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
            audioSource.loop = true; 
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
        audioSource.loop = false; 
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