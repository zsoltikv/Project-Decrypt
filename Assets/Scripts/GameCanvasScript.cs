using TMPro;
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
        foreach (var app in gsm.apps)
        {
            var newApp = Instantiate(appButton);
            newApp.transform.SetParent(appGrid.transform, false);
            newApp.transform.localScale = new Vector3(1, 1, 1);
            newApp.name = app;
            newApp.GetComponentInChildren<TextMeshProUGUI>().text = app;
            if (gsm.completedApps.Contains(app))
            {
                newApp.GetComponentInChildren<TextMeshProUGUI>().text += " [✔]";
            }
            newApp.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("AppIcons/" + app);
            newApp.AddComponent<AppScript>();
        }
    }

}
