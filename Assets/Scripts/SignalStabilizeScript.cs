using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class SignalStabilizeMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform playArea;
    public RectTransform targetZone;
    public RectTransform controlDot;
    public TMPro.TextMeshProUGUI percentageText;
    public GameObject winPanel;

    [Header("Gameplay Settings")]
    public float fillSpeed = 0.15f;
    public float leakSpeed = 0.25f;
    public float targetMoveSpeed = 120f;
    public float radiusMin = 40f;
    public float radiusMax = 80f;

    [Header("Random Movement Timing")]
    public float directionChangeMin = 0.3f;
    public float directionChangeMax = 1.2f;

    [Header("Erratic Movement")]
    public float jitterAmount = 150f;
    public float spiralSpeed = 3f;
    public float zigzagFrequency = 8f;

    private float directionTimer = 0f;
    private Vector2 playSize;
    private Vector2 targetPos;
    private Vector2 targetVelocity;
    private Vector2 baseVelocity;
    private float targetRadius;
    private float progress;
    private bool finished = false;
    private float spiralAngle = 0f;
    private float zigzagTime = 0f;
    private int movementPattern = 0;

    void Start()
    {
        Rect r = playArea.rect;
        playSize = new Vector2(r.width, r.height);

        if (winPanel != null)
            winPanel.SetActive(false);

        progress = 0;
        NewRandomTarget();
    }

    void Update()
    {
        if (finished) return;

        HandleInput();
        MoveTarget();

        bool inside = Vector2.Distance(controlDot.anchoredPosition, targetPos) <= targetRadius * 1.25f;

        if (inside)
            progress += fillSpeed * Time.deltaTime;
        else
            progress -= leakSpeed * Time.deltaTime;

        progress = Mathf.Clamp01(progress);
        percentageText.text = "Signal strength: " + Mathf.RoundToInt(progress * 100f) + "%";

        if (progress >= 1f)
            Win();
    }

    void HandleInput()
    {
        Vector2 screenPos;
        bool hasInput = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            hasInput = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPos = Mouse.current.position.ReadValue();
            hasInput = true;
        }
        else
        {
            return;
        }

        Canvas canvas = playArea.GetComponentInParent<Canvas>();
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceCamera) ? canvas.worldCamera : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            playArea,
            screenPos,
            cam,
            out Vector2 local))
        {
            float halfW = playSize.x / 2f;
            float halfH = playSize.y / 2f;

            local.x = Mathf.Clamp(local.x, -halfW, halfW);
            local.y = Mathf.Clamp(local.y, -halfH, halfH);

            controlDot.anchoredPosition = local;
        }
    }

    void MoveTarget()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomSpeed = Random.Range(targetMoveSpeed * 0.6f, targetMoveSpeed * 1.2f);
            baseVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomSpeed;

            directionTimer = Random.Range(directionChangeMin, directionChangeMax);

            movementPattern = Random.Range(0, 4);
        }

        targetVelocity = baseVelocity;

        switch (movementPattern)
        {
            case 0: 
                zigzagTime += Time.deltaTime * zigzagFrequency;
                Vector2 perpendicular = new Vector2(-baseVelocity.y, baseVelocity.x).normalized;
                targetVelocity += perpendicular * Mathf.Sin(zigzagTime) * jitterAmount * 3f;
                break;

            case 1:
                spiralAngle += Time.deltaTime * spiralSpeed;
                targetVelocity = new Vector2(
                    Mathf.Cos(spiralAngle) * targetMoveSpeed,
                    Mathf.Sin(spiralAngle) * targetMoveSpeed
                );
                break;

            case 2: 
                targetVelocity += new Vector2(
                    Random.Range(-jitterAmount, jitterAmount),
                    Random.Range(-jitterAmount, jitterAmount)
                ) * 5f;
                break;

            case 3:
                float pulseSpeed = (Mathf.Sin(Time.time * 10f) + 1f) * 0.5f;
                targetVelocity *= (0.5f + pulseSpeed * 1.5f);
                break;
        }

        targetVelocity += new Vector2(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount)
        );

        targetPos += targetVelocity * Time.deltaTime;

        float halfW = playSize.x / 2f;
        float halfH = playSize.y / 2f;

        targetPos.x = Mathf.Clamp(targetPos.x, -halfW + targetRadius, halfW - targetRadius);
        targetPos.y = Mathf.Clamp(targetPos.y, -halfH + targetRadius, halfH - targetRadius);

        targetZone.anchoredPosition = targetPos;
        targetZone.sizeDelta = new Vector2(targetRadius * 2.5f, targetRadius * 2.5f);
    }

    void NewRandomTarget()
    {
        targetRadius = Random.Range(radiusMin, radiusMax);

        float halfW = playSize.x / 2f - targetRadius;
        float halfH = playSize.y / 2f - targetRadius;

        targetPos = new Vector2(Random.Range(-halfW, halfW), Random.Range(-halfH, halfH));

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomSpeed = Random.Range(targetMoveSpeed * 0.6f, targetMoveSpeed * 1.2f);
        baseVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomSpeed;
        targetVelocity = baseVelocity;

        movementPattern = Random.Range(0, 4);
        spiralAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        zigzagTime = 0f;
    }

    void Win()
    {
        finished = true;
        if (winPanel != null)
        {
            StartCoroutine(ShowWinPanel());
        }
        else
        {
            if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.completedApps.Contains("SignalStabilize"))
                GameSettingsManager.Instance.completedApps.Add("SignalStabilize");

            StartCoroutine(ReturnToGameScene());
        }
    }

    IEnumerator ShowWinPanel()
    {
        yield return new WaitForSeconds(1f);

        if (winPanel == null)
            yield break;

        winPanel.SetActive(true);

        GameObject[] backButtons = GameObject.FindGameObjectsWithTag("BackButton");
        foreach (GameObject btn in backButtons)
        {
            btn.SetActive(false);
        }


        CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = winPanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        winPanel.transform.localScale = Vector3.one * 0.4f;

        float duration = 0.4f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            cg.alpha = Mathf.Lerp(0f, 1f, t);
            winPanel.transform.localScale = Vector3.Lerp(
                Vector3.one * 0.4f,
                Vector3.one,
                Mathf.Sin(t * Mathf.PI * 0.5f)
            );

            yield return null;
        }

        winPanel.transform.localScale = Vector3.one;
        cg.alpha = 1f;

        if (GameSettingsManager.Instance != null && !GameSettingsManager.Instance.completedApps.Contains("SignalStabilize"))
            GameSettingsManager.Instance.completedApps.Add("SignalStabilize");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("GameScene");
    }

    IEnumerator ReturnToGameScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene");
    }
}