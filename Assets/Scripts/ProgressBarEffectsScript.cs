using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBarEffects : MonoBehaviour
{
    public Image fill;
    private Color originalColor = Color.green;

    void Awake()
    {
        if (fill == null)
        {
            Slider s = GetComponent<Slider>();
            if (s != null && s.fillRect != null)
                fill = s.fillRect.GetComponent<Image>();
        }

        if (fill != null)
            originalColor = fill.color;
        else
            Debug.LogWarning("ProgressBarEffects: fill Image not assigned!");
    }

    public void Pulse()
    {
        StopAllCoroutines();
        StartCoroutine(PulseCoroutine());
    }

    public void FlashError()
    {
        StopAllCoroutines();
        StartCoroutine(FlashErrorCoroutine());
    }

    IEnumerator PulseCoroutine()
    {
        if (fill == null) yield break;

        float duration = 0.4f;
        float elapsed = 0f;
        Color a = Color.Lerp(originalColor, Color.white, 0.08f);

        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 3f, 1f);
            fill.color = Color.Lerp(originalColor, a, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fill.color = originalColor;
    }

    IEnumerator FlashErrorCoroutine()
    {
        if (fill == null) yield break;

        Color prev = fill.color;
        fill.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        fill.color = prev;
    }
}