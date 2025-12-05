/* ----- EXTRA FUNKCIÓK: -----
 * 
 *  - egyedi Cutscene rendszer (Intro, bevezető és Outro)
 *  - Achievement rendszer (26 db előre definiált achievement)
 *  - Leaderboard rendszer (lokális mentéssel, 4 adattaggal) 
 *  - Difficulty rendszer (befolyásolja a minigame-k nehézségét)
 *  - Véletlenszerű jelszó lehetősége
 *  - Több fajta minigame eltérő típusokkal (időkorlátos, logikai, memória, ügyességi)
 *  - Mátrix ihletettségű dizájn
 *  - Haptic feedback rendszer
 *  - Hajszálvékony történet
 *  - UI elemek animációja
 *  - Laptop UI dátum és idő kijelzése (szándékosan 2013 a történet végett)
 *  
 *  - GitHub repository linkje: https://github.com/zsoltikv/Project-Decrypt
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
public class FlashlightShaderController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float flashlightRadius = 0.15f; 
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
    }

    void Update()
    {
        if (materialInstance == null) return;

        Vector2 screenPosition = Vector2.zero;
        bool hasInput = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            hasInput = true;
        }

        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            screenPosition = Mouse.current.position.ReadValue();
            hasInput = true;
        }

        if (hasInput)
        {
            Vector2 normalizedPos = new Vector2(
                screenPosition.x / Screen.width,
                screenPosition.y / Screen.height
            );

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