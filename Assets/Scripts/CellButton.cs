using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellButton : MonoBehaviour
{
    public string value;
    public TextMeshProUGUI valueText;
    private Button button;
    private Image image;
    private CodeLinkerGame gameManager;

    private Color defaultColor;

    public void Setup(string val, CodeLinkerGame gm)
    {
        value = val;
        gameManager = gm;
        valueText.text = val;

        button = GetComponent<Button>();
        image = GetComponent<Image>();
        defaultColor = image.color;

        button.onClick.AddListener(() => gameManager.OnCellSelected(this));
    }

    public void Highlight()
    {
        image.color = new Color(0.2f, 1f, 0.4f);
        gameObject.GetComponent<Button>().enabled = false;
    }

    public void ResetColor()
    {
        image.color = defaultColor;
        gameObject.GetComponent<Button>().enabled = true;
    }

    public void UpdateText()
    {
        valueText.text = value;
    }
}
