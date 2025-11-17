using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    private static TimeDisplay instance;

    private TMP_Text timeText;
    private TMP_Text dateText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (timeText == null)
            timeText = GameObject.Find("TimeText")?.GetComponent<TMP_Text>();
        if (dateText == null)
            dateText = GameObject.Find("DateText")?.GetComponent<TMP_Text>();

        if (timeText != null)
            timeText.text = DateTime.Now.ToString("HH:mm:ss");

        if (dateText != null)
            dateText.text = DateTime.Now.ToString("yyyy.MM.dd");
    }
}