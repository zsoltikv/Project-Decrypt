using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CableManager : MonoBehaviour
{
    public static CableManager Instance;

    public GameObject cableLineTemplate;
    public RectTransform canvasRect;

    private Connector startConnector;
    private RectTransform currentCableLine;
    private List<RectTransform> permanentCables = new List<RectTransform>();

    public TMPro.TMP_Text cableText;
    private int connectedCableCount = 0;

    public GameObject WinPanel;
    public int requiredConnections = 3;

    void Awake()
    {
        Instance = this;

        if (cableLineTemplate != null)
            cableLineTemplate.SetActive(false);
    }

    void Update()
    {
        if (startConnector == null || currentCableLine == null)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            CheckForConnectionOrReset();
            return;
        }

        Vector2 startPos;
        Vector2 mouseLocal;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, startConnector.GetComponent<RectTransform>().position),
            null,
            out startPos
        );

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
            Debug.LogError("No EventSystem found!");
            EndDrag(null);
            return;
        }

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

        GameObject newCable = Instantiate(cableLineTemplate, canvasRect);
        currentCableLine = newCable.GetComponent<RectTransform>();
        currentCableLine.gameObject.SetActive(true);
    }

    public void EndDrag(Connector connector)
    {
        if (startConnector == null || connector == null)
        {
            ResetCable();
            return;
        }

        if (connector != startConnector &&
            startConnector.isLeftSide != connector.isLeftSide &&
            startConnector.connectorID == connector.connectorID)
        {
            SnapCableToConnector(connector);

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

        if (connectedCableCount >= requiredConnections)
            StartCoroutine(WinSequence());
    }

    private void SnapCableToConnector(Connector endConnector)
    {
        if (currentCableLine == null)
            return;

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

        StartCoroutine(Flash(currentCableLine));
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

    private IEnumerator WinSequence()
    {
        StartCoroutine(ShowWinPanel());

        foreach (var cable in permanentCables)
        {
            if (cable != null)
                StartCoroutine(FadeOutAndDestroy(cable));
        }

        permanentCables.Clear();
        WinPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator FadeOutAndDestroy(RectTransform cable)
    {
        CanvasGroup cg = cable.gameObject.AddComponent<CanvasGroup>();

        float duration = 0.4f;
        float time = 0;
        Vector3 initialScale = cable.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            cable.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(cable.gameObject);
    }

    private IEnumerator ShowWinPanel()
    {
        WinPanel.SetActive(true);

        CanvasGroup cg = WinPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = WinPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        WinPanel.transform.localScale = Vector3.one * 0.4f;

        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            cg.alpha = Mathf.Lerp(0f, 1f, t);
            WinPanel.transform.localScale = Vector3.Lerp(
                Vector3.one * 0.4f,
                Vector3.one,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );

            yield return null;
        }
    }

    private IEnumerator Flash(RectTransform cable)
    {
        UnityEngine.UI.Image img = cable.GetComponent<UnityEngine.UI.Image>();
        Color original = img.color;

        img.color = Color.white;

        float time = 0f;
        float duration = 0.2f;

        while (time < duration)
        {
            time += Time.deltaTime;
            img.color = Color.Lerp(Color.white, original, time / duration);
            yield return null;
        }
    }
}