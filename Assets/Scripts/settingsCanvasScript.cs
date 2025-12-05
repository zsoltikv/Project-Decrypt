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

public class settingsCanvasScript : MonoBehaviour
{

    public GameObject settingsPlane;
    public GameObject gamePlane;

    public void OnProceed()
    {
        switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    GameSettingsManager.Instance.maxApps = 10;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    GameSettingsManager.Instance.maxApps = 10;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    GameSettingsManager.Instance.maxApps = 10;
                    break;
                default:
                    break;
            }

            GameSettingsManager.Instance.videoWatched = false;
            GameSettingsManager.Instance.isSet = true;
            GameSettingsManager.Instance.CreatePassword();
            TimerScript.Instance.StartTimer();
            settingsPlane.SetActive(false);
            gamePlane.SetActive(true);
    }
}
