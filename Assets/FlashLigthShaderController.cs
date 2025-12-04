using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
public class FlashlightShaderController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float flashlightRadius = 0.15f; // 0-1 arány
    [SerializeField] private float smoothness = 0.1f;
    [SerializeField] private float smoothSpeed = 10f;
    
    private Image darknessImage;
    private Material materialInstance;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 currentNormalizedPos = new Vector2(0.5f, 0.5f);

    void Start()
    {
        darknessImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        if (darknessImage.material != null)
        {
            materialInstance = new Material(darknessImage.material);
            darknessImage.material = materialInstance;

            materialInstance.SetFloat("_LightRadius", flashlightRadius);
            materialInstance.SetFloat("_LightSoftness", smoothness);
        }
        else
        {
            Debug.LogError("Nincs material az Image-en! Rakd rá a FlashlightCutout shader-t!");
        }
    }

    void Update()
    {
        if (materialInstance == null) return;

        Vector2 screenPosition = Vector2.zero;
        bool hasInput = false;

        // Touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            hasInput = true;
        }
        // Mouse
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPosition = Mouse.current.position.ReadValue();
            hasInput = true;
        }

        if (hasInput)
        {
            // Normalizált 0-1 koordináta a képernyőre
            Vector2 normalizedPos = new Vector2(
                screenPosition.x / Screen.width,
                screenPosition.y / Screen.height
            );

            // Smooth mozgás
            currentNormalizedPos = Vector2.Lerp(currentNormalizedPos, normalizedPos, Time.deltaTime * smoothSpeed);

            materialInstance.SetVector("_LightPos", currentNormalizedPos);
        }
        else
        {
            materialInstance.SetVector("_LightPos", currentNormalizedPos);
        }
    }



    public void SetFlashlightRadius(float radius)
    {
        flashlightRadius = radius;
        if (materialInstance != null)
        {
            materialInstance.SetFloat("_LightRadius", radius);
        }
    }

    public void SetSmoothness(float soft)
    {
        smoothness = soft;
        if (materialInstance != null)
        {
            materialInstance.SetFloat("_LightSoftness", soft);
        }
    }

    void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}