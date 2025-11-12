using UnityEngine;

public class FadeInOnStart : MonoBehaviour
{
    public float fadeDuration = 1.5f; 

    private CanvasGroup canvasGroup;
    private float timer = 0f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (canvasGroup.alpha < 1f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            if (canvasGroup.alpha >= 1f)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}