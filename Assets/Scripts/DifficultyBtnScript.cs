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
using TMPro;

public class DifficultyBtnScript : MonoBehaviour
{
    public void OnDiffSelected()
    {
        switch (gameObject.name)
        {
            case "Easy":
                GameSettingsManager.Instance.SetDifficulty(0);
                break;
            case "Medium":
                GameSettingsManager.Instance.SetDifficulty(1);
                break;
            case "Hard":
                GameSettingsManager.Instance.SetDifficulty(2);
                break;
            default:
                Debug.LogWarning("Ismeretlen nehézség: " + gameObject.name);
                return;
        }

        Transform parent = transform.parent;
        foreach (Transform child in parent)
        {
            var textComp = child.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (textComp == null)
            {
                continue;
            }

            if (child == transform)
            {
                if (!textComp.text.Contains("[X]"))
                    textComp.text += " [X]";
            }
            else
            {
                textComp.text = textComp.text.Replace(" [X]", "");
            }
        }

        Debug.Log("Difficulty set to: " + gameObject.name);
    }
}
