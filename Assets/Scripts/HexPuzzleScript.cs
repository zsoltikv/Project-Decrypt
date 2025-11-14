using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Collections;

public class CodeLinkerGame : MonoBehaviour
{
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

        if (winPanel != null)
            winPanel.SetActive(false);

        if (GameSettingsManager.Instance == null)
        {
            maxRound = 5;
        }
        else
        {
            switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case GameSettingsManager.Difficulty.Easy:
                    maxRound = 5;
                    break;
                case GameSettingsManager.Difficulty.Normal:
                    maxRound = 7;
                    break;
                case GameSettingsManager.Difficulty.Hard:
                    maxRound = 9;
                    break;
                default:
                    break;
            }
        }

        GenerateGrid();
        GenerateNewHash();
        resultText.text = "";
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
            string value = hexChars[Random.Range(0, hexChars.Length)].ToString();
            cb.Setup(value, this);
            cells.Add(cb);
        }
    }

    public void GenerateNewHash()
    {
        List<string> availableChars = new List<string>();
        foreach (Transform cell in gridParent)
            availableChars.Add(cell.GetComponentInChildren<TextMeshProUGUI>().text);

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
            int index = Random.Range(0, pool.Count);
            target.Append(pool[index]);
            pool.RemoveAt(index);
        }

        currentInput = "";
        targetHash = target.ToString();
        targetText.text = $"Target ({currentRound}/{maxRound}): {targetHash}";
    }

    public void OnCellSelected(CellButton cell)
    {
        if (currentInput.Length >= targetHash.Length) return;

        currentInput += cell.value;
        cell.Highlight();

        if (!targetHash.StartsWith(currentInput))
        {
            resultText.text = "<color=#ff4444>Access Denied.</color>";
            Invoke(nameof(ResetRound), 1f);
        }
        else if (currentInput == targetHash && currentRound >= maxRound)
        {
            StartCoroutine(WinAndReturn());
        }
        else if (currentInput == targetHash)
        {
            resultText.text = "<color=#00ff88>Access Granted. </color>";
            currentRound++;
            Invoke(nameof(NextRound), 1f);
        }
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
        currentInput = "";
        resultText.text = "";
        foreach (var c in cells) c.ResetColor();
    }

    void NextRound()
    {
        resultText.text = "";
        foreach (var c in cells)
        {
            c.value = hexChars[Random.Range(0, hexChars.Length)].ToString();
            c.UpdateText();
            c.ResetColor();
        }
        GenerateNewHash();
    }
}