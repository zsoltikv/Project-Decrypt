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
            Debug.Log("Egy végigjátszás alatt csak egyszer lehet menteni!");
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

        Debug.Log($"Saved at: {filePath}");
    }
}