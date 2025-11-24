using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementMenuButton : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OpenAchievements);
        }
    }

    void OpenAchievements()
    {
        Debug.Log("BACK BUTTON MEGNYOMVA");
        SceneManager.LoadScene("AchievementScene");
    }
}