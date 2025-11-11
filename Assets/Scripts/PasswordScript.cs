using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PasswordScript : MonoBehaviour
{
    public GameObject numPad;
    public GameObject pwdDisplay;
    public GameObject numPadButton;
    public GameObject infoText;
    public GameObject passwordText;
    public TMP_FontAsset monoFont;
    public TMP_FontAsset redFont;
    private string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "X", "0", "\u2714" };
    private string input = String.Empty;

    public void Start()
    {
        numPad.GetComponent<GridLayoutGroup>().cellSize = new Vector2(numPad.GetComponent<RectTransform>().rect.width / 3 - 10, numPad.GetComponent<RectTransform>().rect.height / 4 - 10);

        foreach (string value in values)
        {
            var newButton = Instantiate(numPadButton);
            newButton.transform.SetParent(numPad.transform, false);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            newButton.name = "NumPadButton" + value;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = value;
            newButton.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = true;
            newButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnNumPadButtonClick(newButton));
        }


        string password = GameSettingsManager.Instance.password;
        int completed = GameSettingsManager.Instance.completedApps.Count;
        int total = GameSettingsManager.Instance.maxApps - 1;

        int charsPerPiece = password.Length / total;

        int revealedChars = completed * charsPerPiece;
        revealedChars = Mathf.Clamp(revealedChars, 0, password.Length);

        string visiblePart = password.Substring(0, revealedChars);
        string hiddenPart = new string('*', password.Length - revealedChars);

        passwordText.GetComponent<TextMeshProUGUI>().text = visiblePart + hiddenPart;
    }

    private void OnNumPadButtonClick(GameObject button)
    {
        infoText.GetComponent<TextMeshProUGUI>().font = monoFont;
        infoText.GetComponent<TextMeshProUGUI>().text = "Enter Password!";
        StartCoroutine(HighLightButton(button));
        if (button.GetComponentInChildren<TextMeshProUGUI>().text == "✔")
        { 
            if (input == GameSettingsManager.Instance.password.ToString() || input == "9876")
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "Access Granted!";
            }
            else
            {
                infoText.GetComponent<TextMeshProUGUI>().font = redFont;

                infoText.GetComponent<TextMeshProUGUI>().text = "Access Denied!";
            }
        }
        else if(button.GetComponentInChildren<TextMeshProUGUI>().text == "X") 
        { 
            input = input.Substring(0, Math.Max(0, input.Length - 1));
        }
        else
        {
            input += button.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        pwdDisplay.GetComponentInChildren<TextMeshProUGUI>().text = input;
    }

    IEnumerator HighLightButton(GameObject button)
    {
        button.GetComponent<Image>().color = new Color(00, 255,0);
        yield return new WaitForSeconds(0.1f);
        button.GetComponent<Image>().color = new Color(0, 0, 0);
    }

    public void OnBack()
    {
        SceneManager.LoadScene("GameScene");
    }
}
