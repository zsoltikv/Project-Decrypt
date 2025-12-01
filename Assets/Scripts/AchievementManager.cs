using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private List<Achievement> achievements = new List<Achievement>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAchievements()
    {
        achievements.Clear();

        achievements.Add(new Achievement("first_app", "First Steps", "Complete your first mini-game"));
        achievements.Add(new Achievement("complete_all_apps", "Completionist", "Complete all mini-games in a single run"));

        achievements.Add(new Achievement("win_easy", "Casual Hacker", "Win a game on Easy difficulty"));
        achievements.Add(new Achievement("win_normal", "Skilled Operator", "Win a game on Normal difficulty"));
        achievements.Add(new Achievement("win_hard", "Elite Hacker", "Win a game on Hard difficulty"));

        achievements.Add(new Achievement("no_errors", "Flawless", "Complete a game without any errors"));
        achievements.Add(new Achievement("no_errors_hard", "Perfectionist", "Complete a game on Hard difficulty without errors"));

        achievements.Add(new Achievement("byte_master", "Byte Master", "Complete ByteSorter mini-game"));
        achievements.Add(new Achievement("cable_expert", "Cable Expert", "Complete CableConnecting mini-game"));
        achievements.Add(new Achievement("hex_wizard", "Hex Wizard", "Complete HexPuzzle mini-game"));
        achievements.Add(new Achievement("sequence_pro", "Sequence Pro", "Complete SequenceHack mini-game"));
        achievements.Add(new Achievement("rhythm_master", "Rhythm Master", "Complete RythmDecode mini-game"));
        achievements.Add(new Achievement("signal_expert", "Signal Expert", "Complete SignalStabilize mini-game"));
        achievements.Add(new Achievement("frequency_tuner", "Frequency Tuner", "Complete DualFrequency mini-game"));
        achievements.Add(new Achievement("lights_solver", "Lights Solver", "Complete LightsOn mini-game"));
        achievements.Add(new Achievement("malware_defender", "Malware Defender", "Complete MalwareDefender mini-game"));

        achievements.Add(new Achievement("speed_runner", "Speed Runner", "Complete a game in under 5 minutes"));
        achievements.Add(new Achievement("lightning_fast", "Lightning Fast", "Complete a game in under 3 minutes"));

        achievements.Add(new Achievement("save_master", "Save Master", "Use the save feature for the first time"));

        achievements.Add(new Achievement("movie_buff", "Movie Buff", "Watch the intro video"));
    }

    public void UnlockAchievement(string achievementId)
    {
        Achievement achievement = null;
        foreach (Achievement a in achievements)
        {
            if (a.id == achievementId)
            {
                achievement = a;
                break;
            }
        }

        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            SaveAchievements();
            Debug.Log($"Achievement Unlocked: {achievement.title}");
        }
    }

    public bool IsAchievementUnlocked(string achievementId)
    {
        foreach (Achievement a in achievements)
        {
            if (a.id == achievementId)
            {
                return a.isUnlocked;
            }
        }
        return false;
    }

    public List<Achievement> GetAllAchievements()
    {
        return new List<Achievement>(achievements);
    }

    public int GetUnlockedCount()
    {
        int count = 0;
        foreach (Achievement a in achievements)
        {
            if (a.isUnlocked)
                count++;
        }
        return count;
    }

    public int GetTotalCount()
    {
        return achievements.Count;
    }

    void SaveAchievements()
    {
        List<string> unlockedIds = new List<string>();
        foreach (Achievement a in achievements)
        {
            if (a.isUnlocked)
            {
                unlockedIds.Add(a.id);
            }
        }

        AchievementSaveData saveData = new AchievementSaveData();
        saveData.unlockedAchievementIds = unlockedIds.ToArray();

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("AchievementData", json);
        PlayerPrefs.Save();
    }

    public void LoadAchievements()
    {
        foreach (Achievement a in achievements)
        {
            a.isUnlocked = false;
        }

        if (PlayerPrefs.HasKey("AchievementData"))
        {
            string json = PlayerPrefs.GetString("AchievementData");
            AchievementSaveData saveData = JsonUtility.FromJson<AchievementSaveData>(json);

            if (saveData != null && saveData.unlockedAchievementIds != null)
            {
                foreach (string id in saveData.unlockedAchievementIds)
                {
                    foreach (Achievement a in achievements)
                    {
                        if (a.id == id)
                        {
                            a.isUnlocked = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void CheckMiniGameCompletion(string appName)
    {
        UnlockAchievement("first_app");
        switch (appName)
        {
            case "ByteSorter":
                UnlockAchievement("byte_master");
                break;
            case "CableConnecting":
                UnlockAchievement("cable_expert");
                break;
            case "HexPuzzle":
                UnlockAchievement("hex_wizard");
                break;
            case "SequenceHack":
                UnlockAchievement("sequence_pro");
                break;
            case "RythmDecode":
                UnlockAchievement("rhythm_master");
                break;
            case "SignalStabilize":
                UnlockAchievement("signal_expert");
                break;
            case "DualFrequency":
                UnlockAchievement("frequency_tuner");
                break;
            case "LightsOn":
                UnlockAchievement("lights_solver");
                break;
            case "MalwareDefender":
                UnlockAchievement("malware_defender");
                break;
        }

        if (GameSettingsManager.Instance != null &&
            GameSettingsManager.Instance.completedApps.Count >= GameSettingsManager.Instance.maxApps - 1)
        {
            UnlockAchievement("complete_all_apps");
        }
    }

    public void CheckDifficultyWin(GameSettingsManager.Difficulty difficulty)
    {
        switch (difficulty)
        {
            case GameSettingsManager.Difficulty.Easy:
                UnlockAchievement("win_easy");
                break;
            case GameSettingsManager.Difficulty.Normal:
                UnlockAchievement("win_normal");
                break;
            case GameSettingsManager.Difficulty.Hard:
                UnlockAchievement("win_hard");
                break;
        }
    }

    public void CheckErrorCount(int errorCount, GameSettingsManager.Difficulty difficulty)
    {
        if (errorCount == 0)
        {
            UnlockAchievement("no_errors");

            if (difficulty == GameSettingsManager.Difficulty.Hard)
            {
                UnlockAchievement("no_errors_hard");
            }
        }
    }
}