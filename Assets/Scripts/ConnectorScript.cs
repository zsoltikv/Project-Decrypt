using UnityEngine;
using UnityEngine.EventSystems;

public class Connector : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    public int connectorID;
    public bool isLeftSide;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLeftSide)
            CableManager.Instance.BeginDrag(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!isLeftSide)
        {
            CableManager.Instance.EndDrag(this);
        }
    }
}