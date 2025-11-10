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
    [Tooltip("Maximális alfa érték a zajhoz (1 = teljesen fedő, 0 = átlátszó)")]
    public float maxNoiseTransparency = 0.7f;

    [Header("Events")]
    [Tooltip("Esemény, ami megtelik, ha mindkét slider a helyes tartományban van")]
    public UnityEvent onBothCorrect;

    private bool lastBothCorrectState = false;

    void Start()
    {
        // alapérték: ha nincs kép, próbáljunk nem nullreferenciára hivatkozni
        if (noiseImage == null)
            Debug.LogWarning("NoiseImage nincs hozzárendelve a DualFrequencyScannerhez.");
    }

    void Update()
    {
        if (sliderFreq1 == null || sliderFreq2 == null || noiseImage == null)
            return;

        // Ellenőrizze a tartományokat
        bool correct1 = sliderFreq1.value >= correctRange1.x && sliderFreq1.value <= correctRange1.y;
        bool correct2 = sliderFreq2.value >= correctRange2.x && sliderFreq2.value <= correctRange2.y;

        // Számítsuk az alfa visszajelzést: mindkettő közelében tisztul a zaj
        // Közvetlen visszacsatolás: ha a slider középpontjához közelebb, annál kisebb az alfa
        float proximity1 = RangeProximity(sliderFreq1.value, correctRange1);
        float proximity2 = RangeProximity(sliderFreq2.value, correctRange2);

        // Kombinált „tiszta mérték”: ha mindkettő nagyon közel 0 -> teljesen tiszta
        float combined = (proximity1 + proximity2) / 2f; // 0..1 (0 = teljesen helyes, 1 = távol)
        float targetAlpha = Mathf.Clamp01(maxNoiseTransparency * combined);

        // Lerpeljük az alfa változást a simább animációért
        Color c = noiseImage.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 6f);
        noiseImage.color = c;

        // Eseménykiváltás ha mindkettő a jó tartományban van (egyszer minden állapotváltásnál)
        bool bothCorrect = correct1 && correct2;
        if (bothCorrect && !lastBothCorrectState)
        {
            onBothCorrect?.Invoke();
        }
        lastBothCorrectState = bothCorrect;
    }

    // Visszaadja, mennyire van a value a range közelében: 0 = pontosan a range belsejében,
    // 1 = messze (a range-től a teljes 0..1 intervallum figyelembevételével).
    float RangeProximity(float value, Vector2 range)
    {
        if (value >= range.x && value <= range.y)
            return 0f;

        // távolság a legközelebbi range-értékhez
        float nearest = (value < range.x) ? range.x : range.y;
        float dist = Mathf.Abs(value - nearest);

        // normalizáljuk: a maximum lehetséges távolság 0..1 intervallumban a leghatárokat figyelembe véve
        // itt egyszerűsítettük: feltételezve slider 0..1
        return Mathf.Clamp01(dist / 1f);
    }
}