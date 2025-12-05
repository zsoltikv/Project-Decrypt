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
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    [SerializeField]
    public bool RandomPass;
    public bool isSet = false;
    public bool shouldGenAppList = true;

    [Header("Apps")]
    public List<String> apps = new();
    public List<String> completedApps = new();
    public List<String> appList = new();
    public int maxApps;
    public int passLength = 12;
    public string password;

    public bool videoWatched = false;

    public int errorCount;

    public bool saveUsedThisRun = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(int diff)
    {
        currentDifficulty = (Difficulty)diff;
    }

    public void CreatePassword()
    {
        password = "";
        if (!RandomPass)
        {
            password = "246135246";
            return;
        }
        else
        {
            System.Random rand = new System.Random();

            for (int i = 0; i < passLength; i++)
            {
                password += rand.Next(0, 10).ToString();
            }
        }
    }

    public void _Reset()
    {
        isSet = false;
        shouldGenAppList = true;
        videoWatched = false;
        completedApps.Clear();
        appList.Clear();
        currentDifficulty = Difficulty.Normal;
        saveUsedThisRun = false;
        videoWatched = false;
    }
}