using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SequenceHack : MonoBehaviour
{
    [Header("Grid Settings")]
    public List<Button> gridButtons;
    private Color defaultColor = new Color(0.05f, 0.15f, 0.2f, 1f);
    private Color highlightColor = new Color(0f, 1f, 0.8f, 1f);
    private Color wrongColor = new Color(1f, 0.1f, 0.1f, 1f);

    [Header("UI Elements")]
    public TextMeshProUGUI infoText;

    [Header("Visual Effects")]
    public float glowIntensity = 2f;
    public AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;
    private bool isPlayerTurn = false;
    private bool isAnimating = false;
    private int round = 0;
    private int maxRound;

    void Start()
    {
        SetupVisuals();

        if (GameSettingsManager.Instance == null)
            maxRound = 3;
        else
        {
            switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy: maxRound = 3; break;
                case GameSettingsManager.Difficulty.Normal: maxRound = 5; break;
                case GameSettingsManager.Difficulty.Hard: maxRound = 7; break;
            }
        }

        StartCoroutine(StartNewRound());
    }

    void SetupVisuals()
    {
        foreach (var btn in gridButtons)
        {
            Image img = btn.GetComponent<Image>();
            img.color = defaultColor;

            var outline = btn.GetComponent<Outline>();
            if (outline == null)
                outline = btn.gameObject.AddComponent<Outline>();

            outline.effectColor = new Color(0f, 0.8f, 0.7f, 0.5f);
            outline.effectDistance = new Vector2(5, 5);
        }

        if (infoText != null)
        {
            infoText.fontSize = 36;
            infoText.color = new Color(0f, 1f, 0.8f, 1f);
            infoText.fontStyle = FontStyles.Bold;
        }
    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        isAnimating = true;
        playerIndex = 0;
        round++;

        infoText.text = $"<color=#00FFAA>ROUND {round}</color> - ANALYZING SEQUENCE...";
        yield return new WaitForSeconds(1.5f);

        sequence.Add(Random.Range(0, gridButtons.Count));

        yield return StartCoroutine(PlaySequence());

        isAnimating = false;
        isPlayerTurn = true;
        infoText.text = "<color=#FFFF00>INPUT SEQUENCE NOW</color>";
        StartCoroutine(PulseButtonsBorder());
    }

    IEnumerator PlaySequence()
    {
        foreach (int index in sequence)
        {
            Button btn = gridButtons[index];
            Image img = btn.GetComponent<Image>();
            Outline outline = btn.GetComponent<Outline>();

            yield return StartCoroutine(FlashButtonSequence(img, outline));
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FlashButtonSequence(Image img, Outline outline)
    {
        float duration = 0.6f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float pulse = pulseCurve.Evaluate(t) * glowIntensity;

            img.color = Color.Lerp(defaultColor, highlightColor, pulse);
            if (outline != null)
            {
                outline.effectColor = new Color(0f, 1f, 0.8f, pulse);
                outline.effectDistance = new Vector2(3, 3) * (1 + pulse);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        img.color = defaultColor;
        if (outline != null)
        {
            outline.effectColor = new Color(0f, 0.8f, 0.7f, 0.5f);
            outline.effectDistance = new Vector2(5, 5);
        }
    }

    IEnumerator PulseButtonsBorder()
    {
        float elapsed = 0;
        while (isPlayerTurn && !isAnimating)
        {
            float pulse = Mathf.PingPong(elapsed, 1f);

            foreach (var btn in gridButtons)
            {
                var outline = btn.GetComponent<Outline>();
                if (outline != null)
                    outline.effectColor = new Color(0f, 1f, 0.8f, 0.3f + pulse * 0.4f);
            }

            elapsed += Time.deltaTime * 2f;
            yield return null;
        }
    }

    public void OnButtonPressed(int index)
    {
        if (!isPlayerTurn || isAnimating) return;

        Button btn = gridButtons[index];

        if (index == sequence[playerIndex])
        {
            StartCoroutine(FlashButtonCorrect(btn));
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                if (round >= maxRound)
                {
                    isPlayerTurn = false;
                    isAnimating = true;
                    infoText.text = "<color=#00FF00>ACCESS GRANTED - SYSTEM HACKED!</color>";
                    StartCoroutine(WinEffect());
                }
                else
                {
                    isPlayerTurn = false;
                    infoText.text = "<color=#00FF00>SEQUENCE VERIFIED</color>";
                    StartCoroutine(WaitAndNextRound());
                }
            }
        }
        else
        {
            StartCoroutine(FlashButtonWrong(btn));
            isPlayerTurn = false;
            infoText.text = "<color=#FF0000>ACCESS DENIED - SEQUENCE ERROR!</color>";
            StartCoroutine(RestartAfterDelay());
        }
    }


    IEnumerator FlashButtonCorrect(Button btn)
    {
        Image img = btn.GetComponent<Image>();
        Outline outline = btn.GetComponent<Outline>();

        float duration = 0.3f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            img.color = Color.Lerp(defaultColor, highlightColor, 1 - t);
            if (outline != null)
                outline.effectDistance = Vector2.Lerp(new Vector2(5, 5), new Vector2(2, 2), t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        img.color = defaultColor;
    }

    IEnumerator FlashButtonWrong(Button btn)
    {
        Image img = btn.GetComponent<Image>();

        for (int i = 0; i < 3; i++)
        {
            img.color = wrongColor;
            yield return new WaitForSeconds(0.1f);
            img.color = defaultColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator WinEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (var btn in gridButtons)
                btn.GetComponent<Image>().color = new Color(0f, 1f, 0.3f, 1f);

            yield return new WaitForSeconds(0.2f);

            foreach (var btn in gridButtons)
                btn.GetComponent<Image>().color = defaultColor;

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        GameSettingsManager.Instance.completedApps.Add("SequenceHack");
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator WaitAndNextRound()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartNewRound());
    }

    IEnumerator RestartAfterDelay()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var btn in gridButtons)
                btn.GetComponent<Image>().color = wrongColor;

            yield return new WaitForSeconds(0.15f);

            foreach (var btn in gridButtons)
                btn.GetComponent<Image>().color = defaultColor;

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1f);
        sequence.Clear();
        round = 0;
        StartCoroutine(StartNewRound());
    }
}