using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour
{
    public GameObject lbObject;
    public GameObject list;

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

    void Start()
    {
        ClearList();
        LoadListFromFile();
    }

    public void ClearList()
    {
        foreach (Transform child in list.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadListFromFile()
    {
        var headerObj = Instantiate(lbObject, list.transform, false);
        headerObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Name";
        headerObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Time";
        headerObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Error";
        headerObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Difficulty";

        SaveDataList allSave = new SaveDataList();
        string filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            allSave = JsonUtility.FromJson<SaveDataList>(existingJson) ?? new SaveDataList();
        }

        var orderedSaves = allSave.saves.OrderBy(x => x.playTime).ToList();

        foreach (var save in orderedSaves)
        {
            var newObj = Instantiate(lbObject, list.transform, false);
            newObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = save.playerName;
            newObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = save.playTime.ToString("F2") + "s";
            newObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = save.errorCount.ToString();
            newObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = save.difficulty;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(list.transform as RectTransform);
    }

    public void Reset()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, "");
        }

        ClearList();
    }


}
