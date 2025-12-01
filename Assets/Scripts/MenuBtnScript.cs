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

    public void Start()
    {
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
            if (GameSettingsManager.Instance != null)
                GameSettingsManager.Instance.errorCount = 0;

            SceneManager.LoadScene("FirstCutsceneScene");
        }

    }

    public void OnLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    public void OnAchievements()
    {
        SceneManager.LoadScene("AchievementScene");
    }
}