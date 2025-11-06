using UnityEngine;

public class CableManager : MonoBehaviour
{
    public static CableManager Instance;

    public RectTransform cableLine;
    public RectTransform canvasRect;

    private Connector startConnector;

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (startConnector == null) return;

        Vector2 startPos;
        Vector2 mouseLocal;

        // Connector pozíció konvertálása canvas koordinátára
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, startConnector.GetComponent<RectTransform>().position),
            null,
            out startPos
        );

        // Egér pozíció konvertálása canvas koordinátára
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out mouseLocal
        );

        Vector2 dir = mouseLocal - startPos;
        float dist = dir.magnitude;

        cableLine.anchoredPosition = startPos;       // vonal kezdőpontja
        cableLine.sizeDelta = new Vector2(dist, 8f); // hossz változtatása

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        cableLine.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void BeginDrag(Connector connector)
    {
        startConnector = connector;
        cableLine.gameObject.SetActive(true);
    }

    public void EndDrag(Connector connector)
    {
        if (startConnector != null && connector != startConnector &&
            startConnector.isLeftSide != connector.isLeftSide &&
            startConnector.connectorID == connector.connectorID)
        {
            SnapCableToConnector(connector);
            startConnector = null;
            return;
        }

        ResetCable();
    }

    private void SnapCableToConnector(Connector endConnector)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, startConnector.transform.position),
            null,
            out Vector2 startPos
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, endConnector.transform.position),
            null,
            out Vector2 endPos
        );

        Vector2 dir = endPos - startPos;
        float dist = dir.magnitude;

        cableLine.sizeDelta = new Vector2(dist, 8f);
        cableLine.anchoredPosition = startPos;
        cableLine.right = dir;
    }

    private void ResetCable()
    {
        startConnector = null;
        cableLine.gameObject.SetActive(false);
    }
}
