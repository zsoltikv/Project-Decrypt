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
            SceneManager.LoadScene("GameScene");
        }
        if (gameObject.name == "ProceedBtn")
        {
            settingsPlane.SetActive(false);
            gamePlane.SetActive(true);
            switch (gsm.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    gsm.maxApps = 3;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    gsm.maxApps = 5;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    gsm.maxApps = 7;
                    break;
                default:
                    break;
            }
            gsm.isSet = true;
        }
    }
}
