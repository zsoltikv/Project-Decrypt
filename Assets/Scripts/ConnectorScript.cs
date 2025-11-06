using UnityEngine;
using UnityEngine.EventSystems;

public class Connector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int connectorID;        // 1, 2, 3
    public bool isLeftSide;        // bal oldali → true | jobb oldali → false

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLeftSide)
            CableManager.Instance.BeginDrag(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CableManager.Instance.EndDrag(this);
    }
}

