using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BiteSorterScript : MonoBehaviour
{
    public GameObject card;
    public Canvas winCanvas;

    private int gridWith;
    private int gridHeight;
    private GameObject GridObject;
    private string[] bytes = new string[6];
    private List<GameObject> selectedCards = new List<GameObject>();
    public int matchesFound = 0;
    void Start()
    {
        gridWith = Display.main.systemWidth / 6;
        gridHeight = Display.main.systemHeight / 4;
        GridObject = gameObject.transform.GetChild(1).gameObject;
        GridObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(gridWith, gridHeight);

        for (int i = 0; i < 6; i++)
        {
            bytes[i] = System.Convert.ToString(Random.Range(0, 255), 2).PadLeft(8, '0');
        };

        List<string> bytePairs = new List<string>();
        foreach (string b in bytes)
        {
            bytePairs.Add(b);
            bytePairs.Add(b);
        }

        Shuffle(bytePairs);

        for (int i = 0; i < bytePairs.Count; i++)
        {
            var newCard = Instantiate(card, GridObject.transform);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = bytePairs[i];
            newCard.transform.GetChild(0).gameObject.SetActive(false);
            newCard.GetComponent<Button>().onClick.AddListener(() => OnCardClicked(newCard));
        }
    }

    private void OnCardClicked(GameObject newCard)
    {
        if (selectedCards.Contains(newCard) || selectedCards.Count >= 2 || newCard.tag == "Matched")
            return;
        newCard.transform.GetChild(0).gameObject.SetActive(true);
        selectedCards.Add(newCard);
        if (selectedCards.Count == 2)
        {
            if (selectedCards[0].GetComponentInChildren<TextMeshProUGUI>().text == selectedCards[1].GetComponentInChildren<TextMeshProUGUI>().text)
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
            winCanvas.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("GameScene");
        }
    }

    void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}

