using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    [SerializeField]
    public GameObject appButton;
    private int appCount;

    public void OnProceed()
    {
        switch (GameSettingsManager.Instance.currentDifficulty)
        {
            case GameSettingsManager.Difficulty.Easy:
                appCount = 3;
                break;
            case GameSettingsManager.Difficulty.Normal:
                appCount = 5;
                break;
            case GameSettingsManager.Difficulty.Hard:
                appCount = 7;
                break;
            default:
                break;
        }

        for (int i = 0; i < appCount; i++)
        {
            var newApp = Instantiate(appButton);
            newApp.transform.parent = gameObject.transform.GetChild(0).transform;
            newApp.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
