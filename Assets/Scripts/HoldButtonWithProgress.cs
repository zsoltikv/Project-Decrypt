using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HoldButtonWithProgress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Settings")]
    public float holdTime = 2f;

    [Header("UI Elements")]
    public Image fillImage;

    [Header("Event")]
    public UnityEvent onHoldComplete;

    private bool isHolding = false;
    private float timer = 0f;

    private void Update()
    {
        if (!isHolding)
            return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / holdTime);

        if (fillImage != null)
            fillImage.fillAmount = t;

        if (timer >= holdTime)
        {
            isHolding = false;
            onHoldComplete?.Invoke();
            ResetHold();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        timer = 0f;

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHold();
    }

    private void ResetHold()
    {
        isHolding = false;
        timer = 0f;

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }
}
