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


public class RngBtnScript : MonoBehaviour
{
    [SerializeField]
    private GameObject YesBtn;
    [SerializeField]
    private GameObject NoBtn;

    private Text YesLabel;
    private Text NoLabel;

    private void Awake()
    {
        YesLabel = YesBtn.GetComponentInChildren<UnityEngine.UI.Text>();
        NoLabel = NoBtn.GetComponentInChildren<UnityEngine.UI.Text>();
    }

    public void OnYesClicked()
    {
        YesLabel.text = "Yes [X]";
        NoLabel.text = "No";
        GameSettingsManager.Instance.RandomPass = true;
    }

    public void OnNoClicked()
    {
        YesLabel.text = "Yes";
        NoLabel.text = "No [X]";
        GameSettingsManager.Instance.RandomPass = false;
    }
}
