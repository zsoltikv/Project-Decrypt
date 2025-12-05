/* ----- EXTRA FUNKCIÓK: -----
 * 
 *  - egyedi Cutscene rendszer (Intro, bevezető és Outro)
 *  - Achievement rendszer (26 db előre definiált achievement)
 *  - Leaderboard rendszer (lokális mentéssel, 4 adattaggal) 
 *  - Difficulty rendszer (befolyásolja a minigame-k nehézségét)
 *  - Véletlenszerű jelszó lehetősége
 *  - Több fajta minigame eltérő típusokkal (időkorlátos, logikai, memória, ügyességi)
 *  - Mátrix ihletettségű dizájn
 *  - Haptic feedback rendszer
 *  - Hajszálvékony történet
 *  - UI elemek animációja
 *  - Laptop UI dátum és idő kijelzése (szándékosan 2013 a történet végett)
 *  
 *  - GitHub repository linkje: https://github.com/zsoltikv/Project-Decrypt
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LightsOnPuzzle : MonoBehaviour
{
    public ToggleSwitch[] switches;
    private bool[] state;

    public TMP_Text statusText;
    public GameObject winPanel;
    public string gameSceneName = "GameScene";

    private List<int>[] influenceMap;

    void Start()
    {
        state = new bool[switches.Length];
        influenceMap = new List<int>[switches.Length];

        for (int i = 0; i < switches.Length; i++)
        {
            state[i] = false;
            influenceMap[i] = new List<int>();

            int index = i;
            Button btn = switches[i].GetComponentInChildren<Button>();
            switches[i].index = i;
            btn.onClick.AddListener(() => Press(index));
        }

        UpdateStatusText();

        if (winPanel != null)
            winPanel.SetActive(false);

        GenerateInfluenceMap();
        GenerateRandomSolution();
    }

    void GenerateInfluenceMap()
    {
        for (int i = 0; i < switches.Length; i++)
        {
            influenceMap[i].Clear();

            influenceMap[i].Add(i);

            if (i - 1 >= 0) influenceMap[i].Add(i - 1);
            if (i + 1 < switches.Length) influenceMap[i].Add(i + 1);
        }
    }

    private bool isGenerating = false;

    void GenerateRandomSolution()
    {
        isGenerating = true; 

        bool allOn = true;

        do
        {
            for (int i = 0; i < switches.Length; i++)
            {
                state[i] = false;
                switches[i].SetState(false);
            }

            int randomPresses = Random.Range(3, switches.Length + 2);

            for (int i = 0; i < randomPresses; i++)
            {
                int randomIndex = Random.Range(0, switches.Length);
                RandomizeSwitch(randomIndex);
            }

            allOn = true;
            foreach (bool b in state)
            {
                if (!b)
                {
                    allOn = false;
                    break;
                }
            }
        } while (allOn);

        isGenerating = false;
    }

    void RandomizeSwitch(int index)
    {
        foreach (int affected in influenceMap[index])
        {
            state[affected] = !state[affected];
            switches[affected].SetState(state[affected]);
        }
    }

    void Press(int index)
    {
        if (index < 0 || index >= switches.Length)
            return;

        foreach (int affected in influenceMap[index])
        {
            Toggle(affected);
        }

        UpdateStatusText();
        if (!isGenerating)
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

        if (winPanel == null)
        {
            Debug.LogWarning("Win panel not assigned!");
            yield break;
        }

        winPanel.SetActive(true);

        GameObject[] backButtons = GameObject.FindGameObjectsWithTag("BackButton");
        foreach (GameObject btn in backButtons)
        {
            btn.SetActive(false);
        }

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

        if (GameSettingsManager.Instance != null)
        {
            if (GameSettingsManager.Instance.completedApps != null && !GameSettingsManager.Instance.completedApps.Contains("LightsOn"))
            {
                GameSettingsManager.Instance.completedApps.Add("LightsOn");

                // ÚJ: Achievement integráció
                if (AchievementManager.Instance != null)
                {
                    AchievementManager.Instance.CheckMiniGameCompletion("LightsOn");
                }
            }
        }

        SceneManager.LoadScene(gameSceneName);
    }
}