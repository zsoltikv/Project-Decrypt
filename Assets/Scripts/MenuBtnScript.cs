using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MGButtonScript : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "GameBtn")
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
