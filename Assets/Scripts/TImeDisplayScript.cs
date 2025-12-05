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

using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    private static TimeDisplay instance;

    private TMP_Text timeText;
    private TMP_Text dateText;

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
        }
    }

    private void Update()
    {
        if (timeText == null)
            timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
        if (dateText == null)
            dateText = GameObject.Find("DateText")?.GetComponent<TMP_Text>();

        if (timeText != null)
            timeText.text = DateTime.Now.ToString("HH:mm:ss");

        if (dateText != null)
            dateText.text = DateTime.Now.ToString("2013.MM.dd");
    }
}