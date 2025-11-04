using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    [SerializeField]
    public bool RandomPass;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // nem tûnik el scene váltáskor
        }
        else
        {
            Destroy(gameObject); // csak egy példány legyen
        }
    }

    public void SetDifficulty(int diff)
    {
        currentDifficulty = (Difficulty)diff;
    }
}
