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
            resultText.text = "<color=#00ff88>All Rounds Completed.</color>";
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
        yield return new WaitForSeconds(2f);
        GameSettingsManager.Instance.completedApps.Add("HexPuzzle");
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