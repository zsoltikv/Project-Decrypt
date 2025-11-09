using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LightsOnPuzzle : MonoBehaviour
{
    public ToggleSwitch[] switches;
    private bool[] state;

    public TMP_Text statusText;
    public GameObject winPanel;
    public string gameSceneName = "GameScene";

    void Start()
    {
        state = new bool[switches.Length];

        for (int i = 0; i < switches.Length; i++)
        {
            state[i] = false;
            int index = i;

            Button btn = switches[i].GetComponentInChildren<Button>();
            switches[i].index = i;
            btn.onClick.AddListener(() => Press(index));
        }

        UpdateStatusText();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Press(int index)
    {
        Toggle(index);

        if (index - 1 >= 0) Toggle(index - 1);
        if (index + 1 < switches.Length) Toggle(index + 1);

        UpdateStatusText();
        CheckWin();
    }

    void Toggle(int index)
    {
        state[index] = !state[index];
        switches[index].SetState(state[index]);
    }

    void UpdateStatusText()
    {
        int onCount = 0;
        foreach (bool b in state)
            if (b) onCount++;

        statusText.text = $"Turned on switches: {onCount}/{switches.Length}";
    }

    void CheckWin()
    {
        foreach (bool b in state)
            if (!b) return;

        if (winPanel != null)
            StartCoroutine(ShowWinPanel());
    }

    private IEnumerator ShowWinPanel()
    {
        yield return new WaitForSeconds(1f);

        winPanel.SetActive(true);

        CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = winPanel.AddComponent<CanvasGroup>();

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

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(gameSceneName);
    }
}