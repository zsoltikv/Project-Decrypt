using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MGButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public string TextToDisplay = "default";

    void Start()
    {
        Text textChild = GetComponentInChildren<Text>();
        textChild.text = TextToDisplay; 
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "GameBtn")
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
