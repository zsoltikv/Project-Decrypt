using UnityEngine;
using TMPro;

public class DifficultyBtnScript : MonoBehaviour
{
    public void OnDiffSelected()
    {
        switch (gameObject.name)
        {
            case "Easy":
                GameSettingsManager.Instance.SetDifficulty(0);
                break;
            case "Medium":
                GameSettingsManager.Instance.SetDifficulty(1);
                break;
            case "Hard":
                GameSettingsManager.Instance.SetDifficulty(2);
                break;
            default:
                Debug.LogWarning("Ismeretlen nehézség: " + gameObject.name);
                return;
        }

        Transform parent = transform.parent;
        foreach (Transform child in parent)
        {
            var textComp = child.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (textComp == null)
            {
                continue;
            }

            if (child == transform)
            {
                if (!textComp.text.Contains("[X]"))
                    textComp.text += " [X]";
            }
            else
            {
                textComp.text = textComp.text.Replace(" [X]", "");
            }
        }

        Debug.Log("Difficulty set to: " + gameObject.name);
    }
}
