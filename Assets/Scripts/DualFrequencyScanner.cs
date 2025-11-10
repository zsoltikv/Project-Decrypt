using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DualFrequencyScanner : MonoBehaviour
{
    [Header("Slider references")]
    public Slider sliderFreq1;
    public Slider sliderFreq2;

    [Header("Correct ranges (adjustable)")]
    public Vector2 correctRange1 = new Vector2(0.40f, 0.55f);
    public Vector2 correctRange2 = new Vector2(0.70f, 0.85f);

    [Header("UI elements")]
    public Image noiseImage;

    [Header("Noise effect intensity")]
    public float maxNoiseTransparency = 0.7f;

    [Header("Events")]
    public UnityEvent onBothCorrect;

    private bool lastBothCorrectState = false;

    void Start()
    {
        if (noiseImage == null)
            Debug.LogWarning("NoiseImage is not assigned to the DualFrequencyScanner.");
    }

    void Update()
    {
        if (sliderFreq1 == null || sliderFreq2 == null || noiseImage == null)
            return;

        bool correct1 = sliderFreq1.value >= correctRange1.x && sliderFreq1.value <= correctRange1.y;
        bool correct2 = sliderFreq2.value >= correctRange2.x && sliderFreq2.value <= correctRange2.y;

        float proximity1 = RangeProximity(sliderFreq1.value, correctRange1);
        float proximity2 = RangeProximity(sliderFreq2.value, correctRange2);

        float combined = (proximity1 + proximity2) / 2f;
        float targetAlpha = Mathf.Clamp01(maxNoiseTransparency * combined);

        Color c = noiseImage.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 6f);
        noiseImage.color = c;

        bool bothCorrect = correct1 && correct2;
        if (bothCorrect && !lastBothCorrectState)
            onBothCorrect?.Invoke();

        lastBothCorrectState = bothCorrect;
    }

    float RangeProximity(float value, Vector2 range)
    {
        if (value >= range.x && value <= range.y)
            return 0f;

        float nearest = (value < range.x) ? range.x : range.y;
        float dist = Mathf.Abs(value - nearest);
        return Mathf.Clamp01(dist);
    }
}