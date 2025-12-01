using UnityEngine;

public class settingsCanvasScript : MonoBehaviour
{

    public GameObject settingsPlane;
    public GameObject gamePlane;

    public void OnProceed()
    {
        switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    GameSettingsManager.Instance.maxApps = 4;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    GameSettingsManager.Instance.maxApps = 7;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    GameSettingsManager.Instance.maxApps = 10;
                    break;
                default:
                    break;
            }

            GameSettingsManager.Instance.videoWatched = false;
            GameSettingsManager.Instance.isSet = true;
            GameSettingsManager.Instance.CreatePassword();
            TimerScript.Instance.StartTimer();
            settingsPlane.SetActive(false);
            gamePlane.SetActive(true);
    }
}
