using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TMP_Text clockText;

    void Update()
    {
        clockText.text = System.DateTime.Now.ToString("HH:mm:ss\nyyyy.MM.dd");
    }
}