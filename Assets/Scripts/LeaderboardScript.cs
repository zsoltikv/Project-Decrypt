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
    }

    [System.Serializable]
    public class SaveDataList
    {
        public List<SaveData> saves = new List<SaveData>();
    }

    void Start()
    {
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
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(list.transform as RectTransform);
    }


}
