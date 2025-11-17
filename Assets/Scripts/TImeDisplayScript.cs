using UnityEngine;
using TMPro;
using System.Collections;

public class TimeDisplay : MonoBehaviour
{
    private TMP_Text clockText;

    void Start()
    {
        clockText = GetComponent<TMP_Text>();
        StartCoroutine(UpdateClock());
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            clockText.text = System.DateTime.Now.ToString("HH:mm:ss") + "<br>" + System.DateTime.Now.ToString("yyyy.MM.dd");
            yield return new WaitForSeconds(0.5f);
        }
    }
}