/* ----- EXTRA FUNKCIÓK: -----
 * 
 *  - egyedi Cutscene rendszer (Intro, bevezető és Outro)
 *  - Achievement rendszer (26 db előre definiált achievement)
 *  - Leaderboard rendszer (lokális mentéssel, 4 adattaggal) 
 *  - Difficulty rendszer (befolyásolja a minigame-k nehézségét)
 *  - Véletlenszerű jelszó lehetősége
 *  - Több fajta minigame eltérő típusokkal (időkorlátos, logikai, memória, ügyességi)
 *  - Mátrix ihletettségű dizájn
 *  - Haptic feedback rendszer
 *  - Hajszálvékony történet
 *  - UI elemek animációja
 *  - Laptop UI dátum és idő kijelzése (szándékosan 2013 a történet végett)
 *  
 *  - GitHub repository linkje: https://github.com/zsoltikv/Project-Decrypt
 */

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class FirstCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject introPanel;

    private bool cutsceneEnded = false;

    void Start()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void Update()
    {
        if (cutsceneEnded) return;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            SkipCutscene();
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            SkipCutscene();
        }

        if (Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SkipCutscene();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!cutsceneEnded)
            SkipCutscene();
    }

    void SkipCutscene()
    {
        if (cutsceneEnded) return;
        cutsceneEnded = true;

        if (videoPlayer.isPlaying)
            videoPlayer.Stop();

        if (AchievementManager.Instance != null && GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.videoWatched = true;
            AchievementManager.Instance.UnlockAchievement("movie_buff");
        }

        StartCoroutine(FadeOutAndLoad());
    }

    private System.Collections.IEnumerator FadeOutAndLoad()
    {
        if (introPanel == null)
        {
            SceneManager.LoadScene("GameScene");
            yield break;
        }

        CanvasGroup cg = introPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = introPanel.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        introPanel.transform.localScale = Vector3.one;

        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            cg.alpha = Mathf.Lerp(1f, 0f, t);

            introPanel.transform.localScale = Vector3.Lerp(
                Vector3.one,
                Vector3.one * 0.4f,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );

            yield return null;
        }

        cg.alpha = 0f;
        introPanel.transform.localScale = Vector3.one * 0.4f;

        SceneManager.LoadScene("GameScene");
    }
}