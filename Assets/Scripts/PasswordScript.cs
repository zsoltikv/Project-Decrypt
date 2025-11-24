using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AndroidJavaObject vibrator;

public class PasswordScript : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject numPad;
    public GameObject pwdDisplay;
    public GameObject numPadButton;
    public GameObject infoText;
    public GameObject passwordText;
    public GameObject winPanel;
    public GameObject passwordPanel;

    [Header("Fonts")]
    public TMP_FontAsset monoFont;
    public TMP_FontAsset redFont;

    private string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "X", "0", "OK" };
    private string input = String.Empty;
    public CanvasGroup mainCanvasGroup;

    public void Start()
    {
        if (GameSettingsManager.Instance.videoWatched)
        {
            if (passwordPanel != null)
                passwordPanel.SetActive(false);

            if (winPanel != null)
            {
                winPanel.SetActive(true);
                TextMeshProUGUI winText = winPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (winText != null)
                {
                    winText.text = "Access Granted!\nTime Taken: " +
                        TimeSpan.FromSeconds(TimerScript.Instance.time).ToString(@"mm\:ss\.ff");
                }
            }
            return;
        }

        numPad.GetComponent<GridLayoutGroup>().cellSize = new Vector2(
            numPad.GetComponent<RectTransform>().rect.width / 3 - 10,
            numPad.GetComponent<RectTransform>().rect.height / 4 - 10
        );

        foreach (string value in values)
        {
            var newButton = Instantiate(numPadButton);
            newButton.transform.SetParent(numPad.transform, false);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            newButton.name = "NumPadButton" + value;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = value;
            newButton.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = true;
            newButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnNumPadButtonClick(newButton));
        }

        string password = GameSettingsManager.Instance.password;
        int completed = GameSettingsManager.Instance.completedApps.Count;
        int total = GameSettingsManager.Instance.maxApps - 1;
        float progress = total > 0 ? (float)completed / total : 0f;
        int revealedChars = Mathf.RoundToInt(progress * password.Length);
        revealedChars = Mathf.Clamp(revealedChars, 0, password.Length);
        string visiblePart = password.Substring(0, revealedChars);
        string hiddenPart = new string('*', password.Length - revealedChars);

        var tmp = passwordText.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = visiblePart + hiddenPart;
        }
    }

    private void OnNumPadButtonClick(GameObject button)
    {

        string buttonValue = button.GetComponentInChildren<TextMeshProUGUI>().text;

        Vibrate(40);

        if (buttonValue == "OK")
        {
            Vibrate(120);
        }

        infoText.GetComponent<TextMeshProUGUI>().font = monoFont;
        infoText.GetComponent<TextMeshProUGUI>().text = "Enter Password!";
        StartCoroutine(HighLightButton(button));

        if (button.GetComponentInChildren<TextMeshProUGUI>().text == "OK")
        {
            if (input == GameSettingsManager.Instance.password.ToString() || input == "9876")
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "Access Granted!";
                TimerScript.Instance.StopTimer();

                AudioManager.instance.musicDisabled = true;
                AudioManager.instance.StopMusic();

                StartCoroutine(FadeOutAndLoadScene(0.5f, "OutroCutsceneScene"));
            }
            else
            {
                infoText.GetComponent<TextMeshProUGUI>().font = redFont;
                infoText.GetComponent<TextMeshProUGUI>().text = "Access Denied!";
            }
        }
        else if (button.GetComponentInChildren<TextMeshProUGUI>().text == "X")
        {
            input = input.Substring(0, Math.Max(0, input.Length - 1));
        }
        else
        {
            input += button.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        pwdDisplay.GetComponentInChildren<TextMeshProUGUI>().text = input;
    }

    IEnumerator HighLightButton(GameObject button)
    {
        button.GetComponent<Image>().color = new Color(0, 255, 0);
        yield return new WaitForSeconds(0.1f);
        button.GetComponent<Image>().color = new Color(0, 0, 0);
    }

    IEnumerator FadeOutAndLoadScene(float duration, string sceneName)
    {
        if (mainCanvasGroup == null)
        {
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        float startAlpha = mainCanvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            mainCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        mainCanvasGroup.alpha = 0f;
        SceneManager.LoadScene(sceneName);
    }

    public void OnBack()
    {
        StartCoroutine(FadeOutAndLoadScene(0.5f, "GameScene"));
    }

    public void OnMenuClicked()
    {
        GameSettingsManager.Instance._Reset();
        SceneManager.LoadScene("MenuScene");
    }

    void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
    }
    catch (Exception e)
    {
        Debug.Log("Vibration error: " + e.Message);
    }
#endif
    }

}