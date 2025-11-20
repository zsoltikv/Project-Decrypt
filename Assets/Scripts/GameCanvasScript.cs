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

        foreach (Transform child in appGrid.transform)
        {
            Destroy(child.gameObject);
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

        var text = newApp.GetComponentInChildren<TextMeshProUGUI>();
        var rawImage = newApp.GetComponentInChildren<RawImage>(); 

        text.text = appName;

        if (gsm.completedApps.Contains(appName))
        {
            text.text += " [Done]";
            rawImage.color = new Color(0.22f, 0.22f, 0.22f, 1f);  
        }
        else
        {
            rawImage.color = Color.white;
            newApp.AddComponent<AppScript>();
        }

        rawImage.texture = Resources.Load<Texture>("AppIcons/" + appName);
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