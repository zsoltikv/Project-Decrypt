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
using UnityEngine.SceneManagement;
using System.Collections;

public class CanvasScript : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 0.4f;       
    public float scaleStart = 0.4f;         
    public float holdTime = 2f;

    public void OnBack()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void FadeInPanel(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        StartCoroutine(FadePanelRoutine(panel));
    }

    private IEnumerator FadePanelRoutine(GameObject panel)
    {
        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        panel.transform.localScale = Vector3.one * scaleStart;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            cg.alpha = Mathf.Lerp(0f, 1f, t);
            panel.transform.localScale = Vector3.Lerp(
                Vector3.one * scaleStart,
                Vector3.one,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );

            yield return null;
        }

        cg.alpha = 1f;
        panel.transform.localScale = Vector3.one;

        if (holdTime > 0)
            yield return new WaitForSeconds(holdTime);
    }
}