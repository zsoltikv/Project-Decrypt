using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public static TimerScript Instance;

    public float time = 0f;

    private bool isRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        time = 0;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
