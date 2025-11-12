using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SineWaveScanner : MonoBehaviour
{
    [Header("References")]
    public LineRenderer referenceLine;
    public LineRenderer playerLine;
    public Slider amplitudeSlider;
    public Slider frequencySlider;
    public GameObject WinPanel;


    [Header("Wave Settings")]
    public int resolution = 200;
    public float graphWidth = 800f;
    public float graphHeight = 300f;

    [Header("Correct Values")]
    public float targetAmplitude = 1.2f;
    public float targetFrequency = 2.0f;
    public float allowedError = 0.1f;

    private bool triggerOnce = false;

    void Start()
    {
        DrawReferenceWave();
        if (WinPanel != null)
            WinPanel.SetActive(false);
    }

    void Update()
    {
        DrawPlayerWave();

        if (!triggerOnce && IsCloseEnough())
        {
            triggerOnce = true;
            StartCoroutine(WinSequence());
        }
    }

    void DrawReferenceWave()
    {
        referenceLine.positionCount = resolution;
        float step = graphWidth / (resolution - 1);

        for (int i = 0; i < resolution; i++)
        {
            float x = i * step - graphWidth / 2f;
            float y = Mathf.Sin(i * targetFrequency * 2 * Mathf.PI / resolution) * targetAmplitude * (graphHeight / 2f);
            referenceLine.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    void DrawPlayerWave()
    {
        playerLine.positionCount = resolution;
        float step = graphWidth / (resolution - 1);

        float A = amplitudeSlider.value;
        float F = frequencySlider.value;

        for (int i = 0; i < resolution; i++)
        {
            float x = i * step - graphWidth / 2f;
            float y = Mathf.Sin(i * F * 2 * Mathf.PI / resolution) * A * (graphHeight / 2f);
            playerLine.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    bool IsCloseEnough()
    {
        return Mathf.Abs(amplitudeSlider.value - targetAmplitude) < allowedError &&
               Mathf.Abs(frequencySlider.value - targetFrequency) < allowedError;
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (WinPanel != null)
            yield return StartCoroutine(ShowWinPanel());

        yield return new WaitForSecondsRealtime(2f);
        GameSettingsManager.Instance.completedApps.Add("SineWaveScanner");
        SceneManager.LoadScene("GameScene");
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

        WinPanel.transform.localScale = Vector3.one;
        cg.alpha = 1f;
    }
}