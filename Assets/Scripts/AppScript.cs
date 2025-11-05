using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AppScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene($"{gameObject.name}Scene");
    }
}
