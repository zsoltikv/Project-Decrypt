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

using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveScript : MonoBehaviour
{
    [Header("Button")]
    public GameObject inputField;

    [Header("Data")]
    public string nameInput;

    [Header("UI")]
    public GameObject dataSaveFeedback;

    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public float playTime;
        public int errorCount;
        public string difficulty;
    }

    [System.Serializable]
    public class SaveDataList
    {
        public List<SaveData> saves = new List<SaveData>();
    }


    public void OnSave()
    {
        if (GameSettingsManager.Instance.saveUsedThisRun)
        {
            return;
        }

        string filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        SaveDataList allData = new SaveDataList();
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            allData = JsonUtility.FromJson<SaveDataList>(existingJson) ?? new SaveDataList();
        }

        SaveData data = new SaveData
        {
            playerName = inputField.GetComponent<TextMeshProUGUI>().text,
            playTime = TimerScript.Instance.time,
            errorCount = GameSettingsManager.Instance.errorCount,
            difficulty = GameSettingsManager.Instance.currentDifficulty.ToString()
        };

        allData.saves.Add(data);

        string newJson = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, newJson);

        GameSettingsManager.Instance.saveUsedThisRun = true;

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.UnlockAchievement("save_master");

            int saveCount = PlayerPrefs.GetInt("TotalSaveCount", 0);
            saveCount++;
            PlayerPrefs.SetInt("TotalSaveCount", saveCount);
            PlayerPrefs.Save();

            if (saveCount >= 10 && AchievementManager.Instance != null)
            {
                AchievementManager.Instance.UnlockAchievement("archivist");
            }

        }

        if (dataSaveFeedback != null)
        {
            TextMeshProUGUI tmp = dataSaveFeedback.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = "Data saved.";
            }
        }

        Debug.Log($"Saved at: {filePath}");
    }
}