using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleSwitch : MonoBehaviour
{
    public GameObject lever;
    public bool isOn = false;
    public int index;
    public float animationDuration = 0.3f;

    private Coroutine currentAnimation;
    private Image leverImage;

    private Color darkRed = new Color(0.5f, 0f, 0f);
    private Color darkGreen = new Color(0f, 0.5f, 0f);

    void Awake()
    {
        leverImage = lever.GetComponent<Image>();
        UpdateColor();
    }

    public void SetState(bool on)
    {
        isOn = on;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        float targetZ = on ? -45f : 0f;
        currentAnimation = StartCoroutine(PopRotateLever(targetZ));

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (leverImage != null)
            leverImage.color = isOn ? darkGreen : darkRed;
    }

    public void Toggle()
    {
        SetState(!isOn);
    }

    private IEnumerator PopRotateLever(float targetZ)
    {
        Quaternion startRotation = lever.transform.localRotation;
        float overshoot = targetZ + (isOn ? -15f : 15f);

        Quaternion overshootRotation = Quaternion.Euler(0, 0, overshoot);
        Quaternion endRotation = Quaternion.Euler(0, 0, targetZ);

        float halfDuration = animationDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            lever.transform.localRotation = Quaternion.Slerp(startRotation, overshootRotation, elapsed / halfDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            lever.transform.localRotation = Quaternion.Slerp(overshootRotation, endRotation, elapsed / halfDuration);
            yield return null;
        }

        lever.transform.localRotation = endRotation;
        currentAnimation = null;
    }
}