using TMPro;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    [SerializeField]
    public GameObject appButton;
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
            newApp.transform.SetParent(gameObject.transform.GetChild(0).transform, false);
            newApp.transform.localScale = new Vector3(1, 1, 1);
            newApp.name = app;
            newApp.GetComponentInChildren<TextMeshProUGUI>().text = app;
            newApp.AddComponent<AppScript>();
        }
    }

}
