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
            VibrateLong();
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

        private void VibrateLong()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
            {
                AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");

                if (sdkInt >= 26)
                {
                    AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                    AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                        "createOneShot", 400L, 255);
                    vibrator.Call("vibrate", effect);
                }
                else
                {
                    vibrator.Call("vibrate", 400L);
                }
            }
        }
        catch { }
#else
        Handheld.Vibrate();
#endif
    }
}
