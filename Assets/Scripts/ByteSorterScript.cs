using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BiteSorterScript : MonoBehaviour
{
    public GameObject card;
    public GameObject winPanel;
    
    private int gridWith;
    private int gridHeight;
    private GameObject GridObject;
    private string[] bytes = new string[6];
    private List<GameObject> selectedCards = new List<GameObject>();
    public int matchesFound = 0;


    List<string> bytePairs = new List<string>();
    void Start()
    {
        gridWith = Display.main.systemWidth / 8;
        gridHeight = Display.main.systemHeight / 4;
        GridObject = gameObject.transform.GetChild(1).gameObject;
        GridObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gridWith, gridHeight);

        switch (GameSettingsManager.Difficulty.Hard)
        {
            case GameSettingsManager.Difficulty.Easy:
                GameTimerScript.Timer.maxtimer = 100f;
                break;
            case GameSettingsManager.Difficulty.Normal:
                GameTimerScript.Timer.maxtimer = 80f;
                break;
            case GameSettingsManager.Difficulty.Hard:
                GameTimerScript.Timer.maxtimer = 60f;
                break;
        }

        GameTimerScript.SetTimerToMax();
        RandBytes();
        GenCards();

        if (winPanel != null)
            winPanel.SetActive(false); 

    }

    private void RandBytes()
    {
        bytePairs.Clear();
        for (int i = 0; i < 6; i++)
        {
            bytes[i] = System.Convert.ToString(UnityEngine.Random.Range(0, 255), 2).PadLeft(8, '0');
        }

        foreach (string b in bytes)
        {
            bytePairs.Add(b);
            bytePairs.Add(b);
        }

        Shuffle(bytePairs);
    }

    private void GenCards()
    {
        for (int i = 0; i < bytePairs.Count; i++)
        {
            var newCard = Instantiate(card, GridObject.transform);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = bytePairs[i];
            StartCoroutine(HideCards(newCard));
        }
    }

    IEnumerator HideCards(GameObject newCard)
    {
        yield return new WaitForSeconds(2);
        newCard.transform.GetChild(0).gameObject.SetActive(false);
        newCard.GetComponent<Button>().onClick.AddListener(() => OnCardClicked(newCard));
        GameTimerScript.SetTimerToMax();
        GameTimerScript.Run();
    }

    private void Update()
    {
        if (GameTimerScript.Timer.isRunning)
        {
            GameTimerScript.Timer.timer -= Time.deltaTime;
        }
        if(GameTimerScript.Timer.timer < 0)
        {
            Debug.Log("Restart triggered!");
            GameTimerScript.Stop();
            GameTimerScript.Timer.timer = 0; 
            StartCoroutine(Restart());

        }
        GameTimerScript.SetTimerText(TimeSpan.FromSeconds(GameTimerScript.Timer.timer).ToString(@"s\.ff"));
    }

    IEnumerator Restart()
    {
        GameTimerScript.SetInfoText("Out of time. Resetting.");
        yield return new WaitForSeconds(2f);
        foreach (Transform child in GridObject.transform)
        {
            Destroy(child.gameObject);
        }
        GameTimerScript.SetInfoText("Find the pairs!");
        matchesFound = 0;
        RandBytes();
        GenCards();
    }

    private void OnCardClicked(GameObject newCard)
    {
        if (selectedCards.Contains(newCard) || selectedCards.Count >= 2 || newCard.tag == "Matched")
            return;
        newCard.transform.GetChild(0).gameObject.SetActive(true);
        selectedCards.Add(newCard);
        if (selectedCards.Count == 2)
        {
            if (selectedCards[0].GetComponentInChildren<TextMeshProUGUI>().text ==
                selectedCards[1].GetComponentInChildren<TextMeshProUGUI>().text)
            {
                matchesFound++;
                selectedCards[0].GetComponent<Button>().interactable = false;
                selectedCards[1].GetComponent<Button>().interactable = false;
                selectedCards[0].tag = "Matched";
                selectedCards[1].tag = "Matched";
                StartCoroutine(HideCardsAfterDelay(.5f));
            }
            else
            {
                StartCoroutine(HideCardsAfterDelay(1.5f));
            }
        }
    }

    IEnumerator HideCardsAfterDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        foreach (GameObject card in selectedCards)
        {
            if (card.tag != "Matched")
            {
                card.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        selectedCards.Clear();

        if (matchesFound >= 6)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

            yield return StartCoroutine(ShowWinPanel());

            yield return new WaitForSeconds(2f);

            if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.completedApps.Contains("ByteSorter"))
                GameSettingsManager.Instance.completedApps.Add("ByteSorter");

            SceneManager.LoadScene("GameScene");
        }
    }

    IEnumerator ShowWinPanel()
    {
        if (winPanel == null)
            yield break;

        winPanel.SetActive(true);

        GameObject[] backButtons = GameObject.FindGameObjectsWithTag("BackButton");
        foreach (GameObject btn in backButtons)
        {
            btn.SetActive(false);
        }

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

    }

    void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
