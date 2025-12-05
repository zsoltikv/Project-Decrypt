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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MGButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public GameObject settingsPlane;
    [SerializeField]
    public GameObject gamePlane;

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (!GameSettingsManager.Instance.isSet)
            {
                settingsPlane.SetActive(true);
                gamePlane.SetActive(false);
                return;
            }
            else
            {
                settingsPlane.SetActive(false);
                gamePlane.SetActive(true);
                return;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "GameBtn")
        {
            AudioManager.instance.StopMusic();
            if (GameSettingsManager.Instance != null)
                GameSettingsManager.Instance.errorCount = 0;

            SceneManager.LoadScene("FirstCutsceneScene");
        }

    }

    public void OnLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void OnAchievements()
    {
        SceneManager.LoadScene("AchievementScene");
    }
}