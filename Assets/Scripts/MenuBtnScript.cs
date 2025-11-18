using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MGButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public GameObject settingsPlane;
    [SerializeField]
    public GameObject gamePlane;
    GameSettingsManager gsm;

    public void Start()
    {
        gsm = GameSettingsManager.Instance;
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (!GameSettingsManager.Instance.isSet)
            {
                settingsPlane.SetActive(true);
                gamePlane.SetActive(false);
                return;
            }
            else
            {
                settingsPlane.SetActive(false);
                gamePlane.SetActive(true);
                return;
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "GameBtn")
        {
            AudioManager.instance.StopMusic();

            SceneManager.LoadScene("FirstCutsceneScene");
        }

        if (gameObject.name == "ProceedBtn")
        {
            switch (gsm.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    gsm.maxApps = 4;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    gsm.maxApps = 7;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    gsm.maxApps = 10;
                    break;
                default:
                    break;
            }
            gsm.isSet = true;
            gsm.CreatePassword();
            TimerScript.Instance.StartTimer();
            settingsPlane.SetActive(false);
            gamePlane.SetActive(true);
        }
    }

    public void OnLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }
}
