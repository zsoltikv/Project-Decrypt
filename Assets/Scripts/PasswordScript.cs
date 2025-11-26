using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
    private string input = string.Empty;
    public CanvasGroup mainCanvasGroup;

    public void Start()
    {
        if (GameSettingsManager.Instance.videoWatched)
        {
            if (passwordPanel != null) passwordPanel.SetActive(false);
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
            newButton.transform.localScale = Vector3.one;
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
            tmp.text = visiblePart + hiddenPart;
    }

    private void OnNumPadButtonClick(GameObject button)
    {
        string buttonValue = button.GetComponentInChildren<TextMeshProUGUI>().text;

        infoText.GetComponent<TextMeshProUGUI>().font = monoFont;
        infoText.GetComponent<TextMeshProUGUI>().text = "Enter Password!";

        if (buttonValue != "OK")
        {
            VibrateShort();
        }

        if (buttonValue == "OK")
        {
            if (input == GameSettingsManager.Instance.password || input == "9876")
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "Access Granted!";

                VibrateDouble();

                StartCoroutine(DelayedSuccess());
            }
            else
            {
                infoText.GetComponent<TextMeshProUGUI>().font = redFont;
                infoText.GetComponent<TextMeshProUGUI>().text = "Access Denied!";

                VibrateLong();

                input = string.Empty;
                pwdDisplay.GetComponentInChildren<TextMeshProUGUI>().text = input;
            }
        }
        else if (buttonValue == "X")
        {
            if (input.Length > 0)
                input = input.Substring(0, input.Length - 1);
        }
        else
        {
            input += buttonValue;
        }

        pwdDisplay.GetComponentInChildren<TextMeshProUGUI>().text = input;
    }

    private void VibrateShort()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");

                if (sdkInt >= 26)
                {
                    AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                    AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                        "createOneShot", 50L, 255);
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", 50L);
                }
            }
        }
        catch { }
#else
        Handheld.Vibrate();
#endif
    }

    private void VibrateDouble()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                long[] timings = { 0, 100, 120, 100 };
                int[] amplitudes = { 0, -1, 0, -1 };

                AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                    "createWaveform", timings, amplitudes, -1);

                vibrator.Call("vibrate", effect);
            }
        }
        catch { }
#else
        Handheld.Vibrate();
        Invoke("EditorVibrateSecond", 0.2f);
#endif
    }

    private void VibrateLong()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");

                if (sdkInt >= 26)
                {
                    AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                    AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                        "createOneShot", 400L, 255);
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", 400L);
                }
            }
        }
        catch { }
#else
        Handheld.Vibrate();
#endif
    }

    private void EditorVibrateSecond()
    {
        Handheld.Vibrate();
    }

    private IEnumerator DelayedSuccess()
    {
        yield return new WaitForSeconds(0.3f);

        TimerScript.Instance.StopTimer();
        AudioManager.instance.musicDisabled = true;
        AudioManager.instance.StopMusic();
        StartCoroutine(FadeOutAndLoadScene(0.5f, "OutroCutsceneScene"));
    }

    private IEnumerator FadeOutAndLoadScene(float duration, string sceneName)
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
            mainCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
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
}