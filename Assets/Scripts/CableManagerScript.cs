using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CableManager : MonoBehaviour
{
    public static CableManager Instance;

    [Header("References")]
    public GameObject cableLineTemplate;
    public RectTransform canvasRect;
    public GameObject darknessLayer;
    public GameObject lineHolder;

    [Header("Connectors")]
    public List<GameObject> leftConnectors;
    public List<GameObject> rightConnectors;

    private Connector startConnector;
    private RectTransform currentCableLine;
    private List<RectTransform> permanentCables = new List<RectTransform>();
    private Color[] colors = {Color.red, Color.blue, Color.green};

    [Header("UI")]
    public TMPro.TMP_Text cableText;
    private int connectedCableCount = 0;

    public GameObject WinPanel;
    public int requiredConnections = 3;

    [Header("Raycast Settings")]
    public float maxConnectorDistance = 50f;

    void Awake()
    {
        Instance = this;
        if (cableLineTemplate != null) cableLineTemplate.SetActive(false);


        AssignRandomIDs(leftConnectors);
        AssignRandomIDs(rightConnectors);
    }

    void Update()
    {
        if (startConnector == null || currentCableLine == null) return;

        bool pressed = false;
        bool released = false;
        Vector2 screenPos = Vector2.zero;

        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                pressed = true;
            }
            if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                released = true;
            }
        }
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                screenPos = Mouse.current.position.ReadValue();
                pressed = true;
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                screenPos = Mouse.current.position.ReadValue();
                released = true;
            }
        }

        if (!pressed && !released) return;

        if (released)
        {
            CheckForConnectionOrReset(screenPos);
            return;
        }

        UpdateCurrentCable(screenPos);
    }

    private void UpdateCurrentCable(Vector2 screenPos)
    {
        Canvas canvas = canvasRect.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;

        RectTransform startRect = startConnector.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(cam, startRect.position),
            cam,
            out Vector2 startPos
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out Vector2 localPos);

        Vector2 dir = localPos - startPos;
        float dist = dir.magnitude;

        currentCableLine.anchoredPosition = startPos;
        currentCableLine.sizeDelta = new Vector2(dist, 8f);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentCableLine.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void CheckForConnectionOrReset(Vector2 screenPos)
    {
        if (EventSystem.current == null)
        {
            EndDrag(null);
            return;
        }

        Canvas canvas = canvasRect.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Connector targetConnector = null;
        float closestDistance = float.MaxValue;

        foreach (var result in results)
        {
            Connector c = result.gameObject.GetComponent<Connector>();
            if (c != null && c.isLeftSide != startConnector.isLeftSide)
            {
                Vector2 connectorScreenPos = RectTransformUtility.WorldToScreenPoint(cam, c.GetComponent<RectTransform>().position);
                float distance = Vector2.Distance(screenPos, connectorScreenPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetConnector = c;
                }
            }
        }

        if (targetConnector != null && closestDistance <= maxConnectorDistance)
        {
            EndDrag(targetConnector);
        }
        else
        {
            EndDrag(null);
        }
    }

    public void BeginDrag(Connector connector)
    {
        startConnector = connector;
        GameObject newCable = Instantiate(cableLineTemplate, canvasRect);
        newCable.transform.SetParent(lineHolder.transform);
        currentCableLine = newCable.GetComponent<RectTransform>();
        currentCableLine.gameObject.SetActive(true);
        newCable.GetComponent<Image>().color = colors[connector.connectorID - 1];
    }

    public void EndDrag(Connector connector)
    {
        if (startConnector == null || connector == null)
        {
            ResetCable();
            return;
        }

        bool validConnection =
            connector != startConnector &&
            startConnector.isLeftSide != connector.isLeftSide &&
            startConnector.connectorID == connector.connectorID;

        if (validConnection)
        {
            SnapCableToConnector(connector);
            permanentCables.Add(currentCableLine);
            connectedCableCount++;
            UpdateCableText();
        }
        else ResetCable();

        currentCableLine = null;
        startConnector = null;
    }

    private void UpdateCableText()
    {
        if (cableText != null) cableText.text = "Cables connected: " + connectedCableCount;
        if (connectedCableCount >= requiredConnections) StartCoroutine(WinSequence());
    }

    private void SnapCableToConnector(Connector endConnector)
    {
        if (currentCableLine == null) return;

        Canvas canvas = canvasRect.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;

        RectTransform startRect = startConnector.GetComponent<RectTransform>();
        RectTransform endRect = endConnector.GetComponent<RectTransform>();

        Vector2 startScreenPos = RectTransformUtility.WorldToScreenPoint(cam, startRect.position);
        Vector2 endScreenPos = RectTransformUtility.WorldToScreenPoint(cam, endRect.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startScreenPos, cam, out Vector2 startPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, endScreenPos, cam, out Vector2 endPos);

        Vector2 dir = endPos - startPos;
        float dist = dir.magnitude;

        currentCableLine.anchoredPosition = startPos;
        currentCableLine.sizeDelta = new Vector2(dist, 8f);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentCableLine.rotation = Quaternion.Euler(0, 0, angle);

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
        darknessLayer.SetActive(false);
        yield return new WaitForSeconds(3f);
        WinPanel.SetActive(true);

        if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.completedApps.Contains("CableConnecting"))
        {
            GameSettingsManager.Instance.completedApps.Add("CableConnecting");

            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.CheckMiniGameCompletion("CableConnecting");
            }
        }

        GameObject[] backButtons = GameObject.FindGameObjectsWithTag("BackButton");
        foreach (GameObject btn in backButtons)
        {
            btn.SetActive(false);
        }

        foreach (var cable in permanentCables)
            if (cable != null) StartCoroutine(FadeOutAndDestroy(cable));

        permanentCables.Clear();

        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator FadeOutAndDestroy(RectTransform cable)
    {
        CanvasGroup cg = cable.gameObject.AddComponent<CanvasGroup>();
        float duration = 0.4f, time = 0f;
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

    private IEnumerator Flash(RectTransform cable)
    {
        UnityEngine.UI.Image img = cable.GetComponent<UnityEngine.UI.Image>();
        if (img == null) yield break;

        Color original = img.color;
        img.color = Color.white;

        float duration = 0.2f, time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            img.color = Color.Lerp(Color.white, original, time / duration);
            yield return null;
        }

        img.color = original;
    }

    void AssignRandomIDs(List<GameObject>connectors)
    {
        List<int> availableIDs = new List<int> { 1, 2, 3 };
        
        ShuffleList(availableIDs);

        for (int i = 0; i < connectors.Count; i++)
        {
            if (i < availableIDs.Count)
            {
                connectors[i].GetComponent<Connector>().SetId(availableIDs[i]);
                connectors[i].GetComponent<Image>().color = colors[availableIDs[i] - 1];
            }
        }
    }

    // Fisher-Yates shuffle algoritmus List<T>-hez
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
