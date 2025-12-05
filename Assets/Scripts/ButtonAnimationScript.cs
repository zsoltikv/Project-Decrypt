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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonAnimation : MonoBehaviour
{
    private Image image;
    private Color originalColor;

    void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
            Debug.LogError($"{name}: missing Image component!");
        else
            originalColor = image.color;
    }

    public void FlashCorrect()
    {
        StopAllCoroutines();
        StartCoroutine(Flash(Color.green));
    }

    public void FlashWrong()
    {
        StopAllCoroutines();
        StartCoroutine(Flash(Color.red));
    }

    IEnumerator Flash(Color flashColor)
    {
        if (image == null) yield break;

        float duration = 0.25f;
        float half = duration / 2f;
        float t = 0;

        while (t < half)
        {
            float progress = t / half;
            image.color = Color.Lerp(originalColor, flashColor, progress);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < half)
        {
            float progress = t / half;
            image.color = Color.Lerp(flashColor, originalColor, progress);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = originalColor;
    }
}