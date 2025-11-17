using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    [SerializeField]
    public GameObject appButton;
    public GameObject appGrid;
    private GameSettingsManager gsm;

    public void Awake()
    {
        gsm = GameSettingsManager.Instance;
    }

    public void OnEnable()
    {
        if (gsm.shouldGenAppList)
        {
            gsm.appList = GenAppList();
            gsm.shouldGenAppList = false;
        }

        foreach (string app in gsm.appList)
        {
            GenAppIcon(app);
        }
    }

    public void GenAppIcon(string appName)
    {
        var newApp = Instantiate(appButton);
        newApp.transform.SetParent(appGrid.transform, false);
        newApp.transform.localScale = new Vector3(1, 1, 1);
        newApp.name = appName;
        newApp.GetComponentInChildren<TextMeshProUGUI>().text = appName;
        if (gsm.completedApps.Contains(appName))
        {
            newApp.GetComponentInChildren<TextMeshProUGUI>().text += " [✔]";
        }
        newApp.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("AppIcons/" + appName);
        newApp.AddComponent<AppScript>();
    }

    public List<string> GenAppList()
    {
        List<string> randomAppList = new();

        System.Random rand = new();

        randomAppList.Add("SecretFile");

        while (randomAppList.Count != gsm.maxApps)
        {
            int rng = rand.Next(0, 9);
            if (!randomAppList.Contains(gsm.apps[rng]) && gsm.apps[rng] != "SecretFile")
            {
                randomAppList.Add(gsm.apps[rng]);
            }
        }

        return randomAppList;
    }

}
