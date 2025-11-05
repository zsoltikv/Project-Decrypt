using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;
    [SerializeField]
    public bool RandomPass;
    public bool isSet = false;
    [Header("Apps")]
    public List<String> apps = new();
    public List<String> usedApps = new();
    public int maxApps;

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
