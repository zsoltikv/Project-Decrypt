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

using TMPro;
using UnityEngine;

public class GameTimerScript : MonoBehaviour
{
    public static GameTimerScript Timer;
    public GameObject timerText;
    public GameObject infoText;
    public float timer;
    public float maxtimer;
    public bool isRunning = false;

    private void Awake()
    {
        if (Timer == null)
        {
            Timer = this;
        }
    }

    public static void Run()
    {
        if (!Timer.isRunning)
        {
            Timer.isRunning = true;
        }
    }

    public static void Stop()
    {
        if (Timer.isRunning)
        {
            Timer.isRunning = false;
        }
    }

    public static void SetInfoText(string textValue)
    {
        Timer.infoText.GetComponent<TextMeshProUGUI>().text = textValue;
    }

    public static void SetTimerText(string textValue)
    {
        Timer.timerText.GetComponent<TextMeshProUGUI>().text = textValue;
    }

    public static void SetTimerToMax()
    {
        Timer.timer = Timer.maxtimer;
    }
}
