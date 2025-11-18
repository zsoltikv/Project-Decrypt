using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;

public class URPOverridesManager : MonoBehaviour
{
    public static URPOverridesManager Instance;

    public Volume volumeMonochrome;
    private ColorAdjustments colorAdjustments;

    private bool _switch = false;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            volumeMonochrome.profile.TryGet(out colorAdjustments);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSaturation(float value)
    {
        if (colorAdjustments != null)
            colorAdjustments.saturation.Override(value);
    }

    public void ToggleMonochrome()
    {
        _switch = !_switch;
        float target = _switch ? -100f : 0f;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeSaturation(target, 0.8f));
    }

    private IEnumerator FadeSaturation(float targetValue, float duration)
    {
        float startValue = colorAdjustments.saturation.value;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float x = t / duration;
            float smoothT = x < 0.5f
                ? 4f * x * x * x
                : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;

            float lerp = Mathf.Lerp(startValue, targetValue, smoothT);
            colorAdjustments.saturation.Override(lerp);

            yield return null;
        }

        colorAdjustments.saturation.Override(targetValue);
    }

}