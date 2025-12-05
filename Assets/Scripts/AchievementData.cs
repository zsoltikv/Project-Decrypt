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
using UnityEngine;

[Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public bool isUnlocked;

    public Achievement(string id, string title, string description)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.isUnlocked = false;
    }
}

[Serializable]
public class AchievementSaveData
{
    public string[] unlockedAchievementIds;
}