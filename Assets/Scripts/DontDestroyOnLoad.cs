using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad Instance;

    [Header("Persistence setting")]
    public bool dontDestroyOnLoad = false;

    public void Awake()
    {
        if (!dontDestroyOnLoad) return;

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
}
