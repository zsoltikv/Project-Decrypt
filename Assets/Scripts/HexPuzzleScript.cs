using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class CodeLinkerGame : MonoBehaviour
{

    [Header("Fonts")]
    public TMP_FontAsset monoFont;
    public TMP_FontAsset redFont;

    [Header("UI References")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI resultText;
    public GameObject winPanel;

    [Header("Grid Settings")]
    public int gridSize = 6;

    private List<CellButton> cells = new List<CellButton>();
    private string targetHash = "";
    private string currentInput = "";
    private string hexChars = "0123456789ABCDEF";
    private int currentRound = 1;
    private int maxRound = 0;

    void Start()
    {

        GameTimerScript.Stop();
        if (winPanel != null)
            winPanel.SetActive(false);

        if (GameSettingsManager.Instance == null)
        {
            maxRound = 5;
            GameTimerScript.Timer.maxtimer = 30f;
        }
        else
        {
            switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    maxRound = 5;
                    GameTimerScript.Timer.maxtimer = 30f;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    GameTimerScript.Timer.maxtimer = 25f;
                    maxRound = 7;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    GameTimerScript.Timer.maxtimer = 20f;
                    maxRound = 9;
                    break;
                default:
                    break;
            }
        }
        GameTimerScript.SetTimerToMax();
        GenerateGrid();
        GenerateNewHash();
        resultText.text = "";
    }

    void Update()
    {
        if (GameTimerScript.Timer.isRunning)
        {
            GameTimerScript.Timer.timer -= Time.deltaTime;
        }

        if (GameTimerScript.Timer.timer <= 0 && GameTimerScript.Timer.isRunning)
        {
            GameTimerScript.Stop();
            RestartGame();
        }

        GameTimerScript.SetTimerText("Time remaining: " + TimeSpan.FromSeconds(GameTimerScript.Timer.timer).ToString(@"s\.ff"));
    }

    void GenerateGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        cells.Clear();

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject cellObj = Instantiate(cellPrefab, gridParent);
            CellButton cb = cellObj.GetComponent<CellButton>();
            string value = hexChars[UnityEngine.Random.Range(0, hexChars.Length)].ToString();
            cb.Setup(value, this);
            cells.Add(cb);
        }
    }

    public void GenerateNewHash()
    {
        // Gyűjtsük az elérhető karaktereket közvetlenül a cells listából
        List<string> availableChars = new List<string>();
        foreach (var cb in cells)
        {
            if (cb == null) continue;
            // Feltételezem, hogy a CellButton-ban van egy 'value' string mező
            if (!string.IsNullOrEmpty(cb.value))
                availableChars.Add(cb.value.ToUpper()); // normalizáljuk nagybetűre
        }

        if (availableChars.Count == 0)
        {
            Debug.LogError("No available characters in grid — cannot generate hash!");
            return;
        }

        int hashLength = Mathf.Clamp(3 + currentRound, 3, 8);
        if (hashLength > availableChars.Count)
            hashLength = availableChars.Count;

        System.Text.StringBuilder target = new System.Text.StringBuilder();
        List<string> pool = new List<string>(availableChars);

        for (int i = 0; i < hashLength; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            target.Append(pool[index]);
            pool.RemoveAt(index);
        }

        currentInput = "";
        targetHash = target.ToString();
        targetText.text = $"Target ({currentRound}/{maxRound}): {targetHash}";
        GameTimerScript.SetTimerToMax();
        GameTimerScript.Run();
    }

    public void OnCellSelected(CellButton cell)
    {
        if (currentInput.Length >= targetHash.Length) return;

        currentInput += cell.value;
        cell.Highlight();

        if (!targetHash.StartsWith(currentInput))
        {
                Invoke(nameof(rounderror), 0f);
        }
        else if (currentInput == targetHash && currentRound >= maxRound)
        {
            StartCoroutine(WinAndReturn());
        }
        else if (currentInput == targetHash)
        {
            resultText.text = "<color=#00ff88>Access Granted. </color>";

            GameTimerScript.Stop(); // 🔴 TIMER MEGÁLL

            if (currentRound >= maxRound)
            {
                StartCoroutine(WinAndReturn());
            }
            else
            {
                currentRound++;
                Invoke(nameof(NextRound), 1f);
            }
        }
    }

    private void rounderror()
    {
            resultText.font = redFont;
            resultText.text = "Access Denied.";

            GameSettingsManager.Instance.errorCount += 1;
            Invoke(nameof(ResetRound), 1f);
    }

    IEnumerator WinAndReturn()
    {

        yield return new WaitForSeconds(1f);

        if (winPanel != null)
        {
            yield return StartCoroutine(ShowWinPanel());
        }

        yield return new WaitForSeconds(2f);

        if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.completedApps.Contains("HexPuzzle"))
            GameSettingsManager.Instance.completedApps.Add("HexPuzzle");

        SceneManager.LoadScene("GameScene");
    }

    IEnumerator ShowWinPanel()
    {
        winPanel.SetActive(true);

        CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = winPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        winPanel.transform.localScale = Vector3.one * 0.4f;

        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            cg.alpha = Mathf.Lerp(0f, 1f, t);
            winPanel.transform.localScale = Vector3.Lerp(
                Vector3.one * 0.4f,
                Vector3.one,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );

            yield return null;
        }

        winPanel.transform.localScale = Vector3.one;
        cg.alpha = 1f;

        GameObject[] backButtons = GameObject.FindGameObjectsWithTag("BackButton");
        foreach (GameObject btn in backButtons)
        {
            btn.SetActive(false);
        }


    }

    void ResetRound()
    {
        GameTimerScript.Stop();
        currentInput = "";
        resultText.text = "";
        resultText.font = monoFont;
        foreach (var c in cells) c.ResetColor();
    }

    void NextRound()
    {
        resultText.text = "";
        foreach (var c in cells)
        {
            c.value = hexChars[UnityEngine.Random.Range(0, hexChars.Length)].ToString();
            c.UpdateText();
            c.ResetColor();
        }
        GenerateNewHash();
    }

    void RestartGame()
    {
        currentRound = 1;
        currentInput = "";
        resultText.text = "";
        resultText.font = monoFont;

        GenerateGrid();
        GenerateNewHash();

        GameTimerScript.SetTimerToMax();
        GameTimerScript.Run();
    }

}