using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

        winPanel.SetActive(false);
        progress = 0;
        targetPos = targetZone.anchoredPosition;
        NewRandomTarget();
    }

    void Update()
    {
        if (finished) return;

        MoveControlDot();
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

    void MoveControlDot()
    {
        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
            return;

        Vector2 screenPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(playArea, screenPos, null, out Vector2 local);

        float halfW = playSize.x / 2f;
        float halfH = playSize.y / 2f;

        local.x = Mathf.Clamp(local.x, -halfW, halfW);
        local.y = Mathf.Clamp(local.y, -halfH, halfH);

        controlDot.anchoredPosition = local;
    }

    void MoveTarget()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float randomSpeed = Random.Range(targetMoveSpeed * 0.6f, targetMoveSpeed * 1.8f);
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

        if (targetPos.x - targetRadius < -halfW || targetPos.x + targetRadius > halfW)
        {
            targetVelocity.x *= -1;
            baseVelocity.x *= -1;
            baseVelocity = baseVelocity.normalized * Random.Range(targetMoveSpeed * 0.8f, targetMoveSpeed * 1.5f);
            directionTimer = Random.Range(0.1f, 0.5f); 
        }
        if (targetPos.y - targetRadius < -halfH || targetPos.y + targetRadius > halfH)
        {
            targetVelocity.y *= -1;
            baseVelocity.y *= -1;
            baseVelocity = baseVelocity.normalized * Random.Range(targetMoveSpeed * 0.8f, targetMoveSpeed * 1.5f);
            directionTimer = Random.Range(0.1f, 0.5f);
        }

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
        float randomSpeed = Random.Range(targetMoveSpeed * 0.8f, targetMoveSpeed * 1.5f);
        baseVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * randomSpeed;
        targetVelocity = baseVelocity;

        movementPattern = Random.Range(0, 4);
        spiralAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        zigzagTime = 0f;
    }

    void Win()
    {
        finished = true;
        winPanel.SetActive(true);
        GameSettingsManager.Instance.completedApps.Add("SignalStabilizer");
        StartCoroutine(ReturnToGameScene());
    }

    IEnumerator ReturnToGameScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameScene"); 
    }
}