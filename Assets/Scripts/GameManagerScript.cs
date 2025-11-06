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
    public List<String> completedApps = new();
    public int maxApps;
    public int passLength;
    public string password;

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

    public void CreatePassword()
    {
        if (!RandomPass)
        {
            password = "246135";
            return;
        }
        else
        {
            System.Random rand = new System.Random();
            switch (GameSettingsManager.Instance.currentDifficulty)
            {
                case Difficulty.Easy:
                    passLength = 10;
                    break;
                case Difficulty.Normal:
                    passLength = 14;
                    break;
                case Difficulty.Hard:
                    passLength = 20;
                    break;
            }
            for (int i = 0; i < passLength; i++)
            {
                password += rand.Next(0, 10);
            }
        }

    }
}
