using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    [Header("Button")]
    public GameObject saveButton;

    public GameObject inputField;

    [Header("Data")]
    public string nameInput;

    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public float playTime;
    }

    [System.Serializable]
    public class SaveDataList
    {
        public List<SaveData> saves = new List<SaveData>();
    }


    public void OnSave()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        // Load existing saves (if any)
        SaveDataList allData = new SaveDataList();
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            allData = JsonUtility.FromJson<SaveDataList>(existingJson) ?? new SaveDataList();
        }

        // Create new save entry
        SaveData data = new SaveData
        {
            playerName = inputField.GetComponent<TextMeshProUGUI>().text,
            playTime = TimerScript.Instance.time
        };

        // Add to list
        allData.saves.Add(data);

        // Write back the updated list
        string newJson = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, newJson);

        Debug.Log($"Saved at: {filePath}");
    }
}
