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

public class Connector : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    public int connectorID;
    public bool isLeftSide;

    public bool isFilled = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLeftSide)
            CableManager.Instance.BeginDrag(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!isLeftSide)
        {
            CableManager.Instance.EndDrag(this);
        }
    }

    public void SetId(int id)
    {
        connectorID = id;
    }
}