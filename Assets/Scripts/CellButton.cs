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
using TMPro;

public class CellButton : MonoBehaviour
{
    public string value;
    public TextMeshProUGUI valueText;
    private Button button;
    private Image image;
    private CodeLinkerGame gameManager;

    private Color defaultColor;

    public void Setup(string val, CodeLinkerGame gm)
    {
        value = val;
        gameManager = gm;
        valueText.text = val;

        button = GetComponent<Button>();
        image = GetComponent<Image>();
        defaultColor = image.color;

        button.onClick.AddListener(() => gameManager.OnCellSelected(this));
    }

    public void Highlight()
    {
        image.color = new Color(0.2f, 1f, 0.4f);
        valueText.color = Color.black;
        gameObject.GetComponent<Button>().enabled = false;
    }

    public void ResetColor()
    {
        image.color = defaultColor;
        valueText.color = Color.white;
        gameObject.GetComponent<Button>().enabled = true;
    }

    public void UpdateText()
    {
        valueText.text = value;
    }
}
