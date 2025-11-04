using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MGButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public Canvas settingsPlane;
    [SerializeField]
    public Canvas gamePlane;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "GameBtn")
        {
            SceneManager.LoadScene("GameScene");
        }
        if (gameObject.name == "ProceedBtn")
        {
            settingsPlane.enabled = false;
            gamePlane.enabled = true;
        }
    }
}
