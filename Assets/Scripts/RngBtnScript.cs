using UnityEngine;
using UnityEngine.UI;


public class RngBtnScript : MonoBehaviour
{
    [SerializeField]
    private GameObject YesBtn;
    [SerializeField]
    private GameObject NoBtn;

    private Text YesLabel;
    private Text NoLabel;

    private void Awake()
    {
        YesLabel = YesBtn.GetComponentInChildren<UnityEngine.UI.Text>();
        NoLabel = NoBtn.GetComponentInChildren<UnityEngine.UI.Text>();
    }

    public void OnYesClicked()
    {
        YesLabel.text = "Yes [X]";
        NoLabel.text = "No";
        GameSettingsManager.Instance.RandomPass = true;
    }

    public void OnNoClicked()
    {
        YesLabel.text = "Yes";
        NoLabel.text = "No [X]";
        GameSettingsManager.Instance.RandomPass = false;
    }
}
