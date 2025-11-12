using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RhythmGame : MonoBehaviour
{
    [Header("UI Components")]
    public Slider progressBar;
    public TMP_Text nextIndicator;
    public Button[] buttons;

    [Header("Timer UI")]
    public TMP_Text timerText;

    [Header("Effects (optional)")]
    public ButtonAnimation[] buttonAnimations;
    public ProgressBarEffects progressEffects;
    public TextEffects textEffects;
    public GameObject winPanel;

    [Header("Gameplay Settings")]
    public float reactionTime = 1.5f;
    public float progressIncrement;
    public float progressDecrement;

    private int currentButtonIndex;
    private float timer;
    private int previousButtonIndex = -1;

    void Start()
    {
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                if (buttons[i] != null)
                    buttons[i].onClick.AddListener(() => ButtonPressed(index));
            }
        }
        else
        {
            Debug.LogError("RhythmGame: Buttons array is null or empty in inspector.");
        }

        if (buttonAnimations != null && buttonAnimations.Length != buttons.Length)
            Debug.LogWarning("buttonAnimations length doesn't match buttons length.");

        NextStep();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timerText != null)
            timerText.text = timer.ToString("Time remaining: 0.000");

        if (timer <= 0f)
            Miss();
    }


    void NextStep()
    {
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("No buttons assigned.");
            return;
        }

        do
        {
            currentButtonIndex = Random.Range(0, buttons.Length);
        } while (buttons.Length > 1 && currentButtonIndex == previousButtonIndex);

        previousButtonIndex = currentButtonIndex;

        TMP_Text buttonText = buttons[currentButtonIndex].GetComponentInChildren<TMP_Text>();
        nextIndicator.text = buttonText != null ? "Press: " + buttonText.text : "Press a button!";

        timer = reactionTime;

        if (buttonAnimations != null && currentButtonIndex < buttonAnimations.Length && buttonAnimations[currentButtonIndex] != null)
            buttonAnimations[currentButtonIndex].FlashCorrect();
        else if (buttons[currentButtonIndex] != null)
            StartCoroutine(ScaleButtonHint(buttons[currentButtonIndex].transform));
    }

    IEnumerator ScaleButtonHint(Transform t)
    {
        Vector3 start = t.localScale;
        Vector3 target = start * 1.06f;
        float dur = 0.12f;
        float e = 0f;

        while (e < dur)
        {
            t.localScale = Vector3.Lerp(start, target, e / dur);
            e += Time.deltaTime;
            yield return null;
        }

        t.localScale = start;
    }

    public void ButtonPressed(int index)
    {
        if (buttons == null) return;

        if (index == currentButtonIndex)
        {
            progressBar.value = Mathf.Clamp01(progressBar.value + progressIncrement);

            if (buttonAnimations != null && index < buttonAnimations.Length && buttonAnimations[index] != null)
                buttonAnimations[index].FlashCorrect();
            if (progressEffects != null)
                progressEffects.Pulse();
            if (textEffects != null)
                textEffects.Pulse();

            if (progressBar.value >= 1f)
                Win();

            StartCoroutine(NextStepDelayed());
        }
        else
        {
            progressBar.value = Mathf.Clamp01(progressBar.value - progressDecrement);

            if (buttonAnimations != null && index < buttonAnimations.Length && buttonAnimations[index] != null)
                buttonAnimations[index].FlashWrong();
            if (progressEffects != null)
                progressEffects.FlashError();
            if (textEffects != null)
                textEffects.Shake();

            NextStep();
        }
    }

    IEnumerator NextStepDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        NextStep();
    }

    void Miss()
    {
        progressBar.value = Mathf.Clamp01(progressBar.value - progressDecrement);

        if (progressEffects != null)
            progressEffects.FlashError();
        if (textEffects != null)
            textEffects.Shake();

        NextStep();
    }

    void Win()
    {
        StartCoroutine(ShowWinPanelAndReturn());
    }

    IEnumerator ShowWinPanelAndReturn()
    {
        yield return new WaitForSeconds(1f);

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (GameSettingsManager.Instance != null)
            GameSettingsManager.Instance.completedApps.Add("RythymDecode");

        SceneManager.LoadScene("GameScene");
    }

}