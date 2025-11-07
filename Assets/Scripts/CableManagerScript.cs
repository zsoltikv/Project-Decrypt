using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // szükséges a Scene váltáshoz

public class CableManager : MonoBehaviour
{
    public static CableManager Instance;
    public GameObject cableLineTemplate; // Ez egy inaktív UI Image lesz a Canvas-on
    public RectTransform canvasRect;

    private Connector startConnector;
    private RectTransform currentCableLine;
    private List<RectTransform> permanentCables = new List<RectTransform>();

    public TMPro.TMP_Text cableText;
    private int connectedCableCount = 0;

    public GameObject WinPanel;     // A UI panel, ami megjelenik (Power Restored)
    public int requiredConnections = 3;  // hány kábelt kell csatlakoztatni

    void Awake()
    {
        Instance = this;
        // Template-et elrejtjük
        if (cableLineTemplate != null)
            cableLineTemplate.SetActive(false);
    }

    void Update()
    {
        if (startConnector == null || currentCableLine == null) return;

        // Ellenőrizzük, hogy felengedték-e az egérgombot
        if (Input.GetMouseButtonUp(0))
        {
            CheckForConnectionOrReset();
            return;
        }

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
        currentCableLine.anchoredPosition = startPos;
        currentCableLine.sizeDelta = new Vector2(dist, 8f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentCableLine.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void CheckForConnectionOrReset()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("Nincs EventSystem!");
            EndDrag(null);
            return;
        }

        // Raycast az egér pozícióján
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Connector targetConnector = null;
        foreach (var result in results)
        {
            Connector connector = result.gameObject.GetComponent<Connector>();
            if (connector != null && !connector.isLeftSide)
            {
                targetConnector = connector;
                break;
            }
        }

        EndDrag(targetConnector);
    }

    public void BeginDrag(Connector connector)
    {
        startConnector = connector;

        // Új kábel létrehozása a template alapján
        GameObject newCable = Instantiate(cableLineTemplate, canvasRect);
        currentCableLine = newCable.GetComponent<RectTransform>();
        currentCableLine.gameObject.SetActive(true);
    }

    public void EndDrag(Connector connector)
    {
        // Ha nincs startConnector, vagy null a connector, reset
        if (startConnector == null || connector == null)
        {
            ResetCable();
            return;
        }

        // Ellenőrizzük, hogy megfelelő kapcsolat-e
        if (connector != startConnector &&
            startConnector.isLeftSide != connector.isLeftSide &&
            startConnector.connectorID == connector.connectorID)
        {
            SnapCableToConnector(connector);
            // Kábelt permanenssé tesszük
            permanentCables.Add(currentCableLine);

            connectedCableCount++;
            UpdateCableText();

            currentCableLine = null;
            startConnector = null;
            return;
        }

        ResetCable();
    }

    private void UpdateCableText()
    {
        if (cableText != null)
            cableText.text = "Cables Connected: " + connectedCableCount;

        // Ha elérte a maximumot → győzelem
        if (connectedCableCount >= requiredConnections)
            StartCoroutine(WinSequence());
    }

    private void SnapCableToConnector(Connector endConnector)
    {
        if (currentCableLine == null) return;

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
        currentCableLine.sizeDelta = new Vector2(dist, 8f);
        currentCableLine.anchoredPosition = startPos;
        currentCableLine.right = dir;
    }

    private void ResetCable()
    {
        if (currentCableLine != null)
        {
            Destroy(currentCableLine.gameObject);
            currentCableLine = null;
        }
        startConnector = null;
    }

    private System.Collections.IEnumerator WinSequence()
    {
        if (WinPanel != null)
            WinPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        // Scene visszatöltése
        SceneManager.LoadScene("GameScene");
    }

}