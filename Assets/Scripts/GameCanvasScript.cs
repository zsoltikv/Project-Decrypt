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
        for (int i = 0; i < gsm.maxApps; i++)
        {
            var newApp = Instantiate(appButton);
            newApp.transform.SetParent(appGrid.transform, false);
            newApp.transform.localScale = new Vector3(1, 1, 1);
            newApp.name = gsm.apps[i];
            newApp.GetComponentInChildren<TextMeshProUGUI>().text = gsm.apps[i];
            if (gsm.completedApps.Contains(gsm.apps[i]))
            {
                newApp.GetComponentInChildren<TextMeshProUGUI>().text += " [✔]";
            }
            newApp.GetComponentInChildren<RawImage>().texture = Resources.Load<Texture>("AppIcons/" + gsm.apps[i]);
            newApp.AddComponent<AppScript>();
        }
    }

}
