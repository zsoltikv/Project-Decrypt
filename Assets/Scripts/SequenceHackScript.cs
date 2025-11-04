using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SequenceHack : MonoBehaviour
{
    [Header("Grid Settings")]
    public List<Button> gridButtons; // A 3x3 mezõk listája (Inspectorban add hozzá)
    private Color defaultColor;

    [Header("UI Elements")]
    public TextMeshProUGUI infoText;

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;
    private bool isPlayerTurn = false;
    private bool isAnimating = false;
    private int round = 0;
    private int maxRound;

    void Start()
    {
        defaultColor = gridButtons[0].GetComponent<Image>().color;
        if(GameSettingsManager.Instance == null)
        {
            maxRound = 3;
        }
        else
        {
            switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    maxRound = 3;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    maxRound = 5;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    maxRound = 7;
                    break;
                default:
                    break;
            }
        }

        StartCoroutine(StartNewRound());

    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        isAnimating = true;
        playerIndex = 0;
        round++;

        infoText.text = "Round " + round + " - Watch carefully!";
        yield return new WaitForSeconds(1f);

        // új elem hozzáadása a szekvenciához
        sequence.Add(Random.Range(0, gridButtons.Count));

        // lejátszás (villogtatás)
        yield return StartCoroutine(PlaySequence());

        isAnimating = false;
        isPlayerTurn = true;
        infoText.text = "Now repeat!";
    }

    IEnumerator PlaySequence()
    {
        foreach (int index in sequence)
        {
            Button btn = gridButtons[index];
            Image img = btn.GetComponent<Image>();

            img.color = new Color(Random.value, Random.value, Random.value, 1);
            yield return new WaitForSeconds(0.5f);
            img.color = defaultColor;
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void OnButtonPressed(int index)
    {
        if (!isPlayerTurn || isAnimating) return;

        Button btn = gridButtons[index];
        StartCoroutine(FlashButton(btn));

        // helyes gomb?
        if (index == sequence[playerIndex])
        {
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                // ha elérte a maxRound-ot (azaz nyert)
                if (round >= maxRound)
                {
                    isPlayerTurn = false;
                    isAnimating = true;           // letiltjuk a további inputot
                    infoText.text = "Congratulations! You've hacked the system!";
                    StartCoroutine(WinAndReturn()); // itt történik a várakozás + scene load
                }
                else
                {
                    // normál eset: jön a következõ kör
                    isPlayerTurn = false;
                    infoText.text = "Good job!";
                    StartCoroutine(WaitAndNextRound());
                }
            }
        }
        else
        {
            // hibázott
            isPlayerTurn = false;
            infoText.text = "Wrong sequence!";
            StartCoroutine(RestartAfterDelay());
        }
    }

    IEnumerator WinAndReturn()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator FlashButton(Button btn)
    {
        Image img = btn.GetComponent<Image>();
        img.color = new Color(Random.value, Random.value, Random.value, 1);
        yield return new WaitForSeconds(0.2f);
        img.color = defaultColor;
    }

    IEnumerator WaitAndNextRound()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartNewRound());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        sequence.Clear();
        round = 0;
        StartCoroutine(StartNewRound());
    }
}
