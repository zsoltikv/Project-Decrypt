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
using TMPro;
using System.Collections;

public class TextEffects : MonoBehaviour
{
    private TMP_Text text;
    private Vector3 startScale;
    private Vector3 startPos;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        startScale = transform.localScale;
        startPos = transform.localPosition;
        if (text == null) Debug.LogWarning($"{name}: no TMP_Text found!");
    }

    public void Pulse()
    {
        StopAllCoroutines();
        StartCoroutine(PulseAnimation());
    }

    public void Shake()
    {
        StopAllCoroutines();
    }

    IEnumerator PulseAnimation()
    {
        float duration = 0.4f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float s = 1f + Mathf.Sin(elapsed * Mathf.PI * 2f) * 0.12f;
            transform.localScale = startScale * s;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = startScale;
    }
}