using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;
    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty currentDifficulty;

    [SerializeField]
    public bool RandomPass;
    public bool isSet = false;
    public bool shouldGenAppList = true;

    [Header("Apps")]
    public List<String> apps = new();
    public List<String> completedApps = new();
    public List<String> appList = new();
    public int maxApps;
    public int passLength = 12;
    public string password;

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

    public void SetDifficulty(int diff)
    {
        currentDifficulty = (Difficulty)diff;
    }

    public void CreatePassword()
    {
        password = "";
        if (!RandomPass)
        {
            password = "246135246135";
            return;
        }
        else
        {
            System.Random rand = new System.Random();

            for (int i = 0; i < passLength; i++)
            {
                password += rand.Next(0, 10).ToString();
            }
        }
    }

    public void _Reset()
    {
        isSet = false;
        shouldGenAppList = true;
        completedApps.Clear();
        appList.Clear();
        currentDifficulty = Difficulty.Normal;
    }
}