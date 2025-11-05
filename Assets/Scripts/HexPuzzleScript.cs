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
        // Collect all available characters from the grid
        List<string> availableChars = new List<string>();
        foreach (Transform cell in gridParent)
        {
            availableChars.Add(cell.GetComponentInChildren<TextMeshProUGUI>().text);
        }

        if (availableChars.Count == 0)
        {
            Debug.LogError("⚠️ No available characters in grid — cannot generate hash!");
            return;
        }

        // Decide how long the hash should be
        int hashLength = Mathf.Clamp(3 + currentRound, 3, 8);
        if (hashLength > availableChars.Count)
        {
            // Avoid impossible hash (longer than available cells)
            hashLength = availableChars.Count;
        }

        // Build target hash *only from available pool*
        System.Text.StringBuilder target = new System.Text.StringBuilder();
        List<string> pool = new List<string>(availableChars);

        for (int i = 0; i < hashLength; i++)
        {
            // pick random index and remove it from pool (so duplicates can't exceed actual availability)
            int index = Random.Range(0, pool.Count);
            target.Append(pool[index]);
            pool.RemoveAt(index);
        }

        // Set up the new round
        currentInput = "";
        targetHash = target.ToString();
        targetText.text = $"TARGET ({currentRound}/{maxRound}): {targetHash}";
    }

    public void OnCellSelected(CellButton cell)
    {
        if (currentInput.Length >= targetHash.Length) return;

        currentInput += cell.value;
        cell.Highlight();

        // check progress
        if (!targetHash.StartsWith(currentInput))
        {
            resultText.text = "<color=#ff4444>ACCESS DENIED</color>";
            Invoke(nameof(ResetRound), 1f);
        }
        else if (currentInput == targetHash && currentRound >= maxRound)
        {
            resultText.text = "<color=#00ff88>ALL ROUNDS COMPLETED!</color>";
            StartCoroutine(WinAndReturn());
        }
        else if (currentInput == targetHash)
        {
            resultText.text = "<color=#00ff88>ACCESS GRANTED</color>";
            currentRound++;
            Invoke(nameof(NextRound), 1f);
        }
    }

    IEnumerator WinAndReturn()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene");
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
