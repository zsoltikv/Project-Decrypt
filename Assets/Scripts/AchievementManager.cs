/* ----- EXTRA FUNKCIÓK: -----
 * 
 *  - egyedi Cutscene rendszer (Intro, bevezető és Outro)
 *  - Achievement rendszer (26 db előre definiált achievement)
 *  - Leaderboard rendszer (lokális mentéssel, 4 adattaggal) 
 *  - Difficulty rendszer (befolyásolja a minigame-k nehézségét)
 *  - Véletlenszerű jelszó lehetősége
 *  - Több fajta minigame eltérő típusokkal (időkorlátos, logikai, memória, ügyességi)
 *  - Mátrix ihletettségű dizájn
 *  - Haptic feedback rendszer
 *  - Hajszálvékony történet
 *  - UI elemek animációja
 *  - Laptop UI dátum és idő kijelzése (szándékosan 2013 a történet végett)
 *  
 *  - GitHub repository linkje: https://github.com/zsoltikv/Project-Decrypt
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private float uninterruptedPlayTime = 0f;
    private const float ONE_HOUR = 3600f;
    private const float THREE_HOURS = 10800f;

    private bool[] daysPlayed = new bool[7];
    private const string WEEKLY_STREAK_KEY = "WeeklyStreak";

    private List<Achievement> achievements = new List<Achievement>();

    void Update()
    {
        if (!IsAchievementUnlocked("play_3h"))
        {
            uninterruptedPlayTime += Time.deltaTime;

            if (uninterruptedPlayTime >= THREE_HOURS)
                UnlockAchievement("play_3h");
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAchievements();
            LoadAchievements();
            LoadWeeklyStreak();
            CheckWeeklyStreak();
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

        achievements.Add(new Achievement("byte_master", "Byte Master", "Complete ByteSorter mini-game"));
        achievements.Add(new Achievement("cable_expert", "Crewmate Certified Electrician", "Complete CableConnecting mini-game"));
        achievements.Add(new Achievement("hex_wizard", "Hex Wizard", "Complete HexPuzzle mini-game"));
        achievements.Add(new Achievement("sequence_pro", "Sequence Pro", "Complete SequenceHack mini-game"));
        achievements.Add(new Achievement("rhythm_master", "Rhythm Master", "Complete RythmDecode mini-game"));
        achievements.Add(new Achievement("signal_expert", "Signal Expert", "Complete SignalStabilize mini-game"));
        achievements.Add(new Achievement("frequency_tuner", "Frequency Tuner", "Complete DualFrequency mini-game"));
        achievements.Add(new Achievement("lights_solver", "Lights Solver", "Complete LightsOn mini-game"));
        achievements.Add(new Achievement("malware_defender", "Unauthorized Access Denied", "Complete MalwareDefender mini-game"));

        achievements.Add(new Achievement("win_easy", "Casual Hacker", "Win a game on Easy difficulty"));
        achievements.Add(new Achievement("win_normal", "Skilled Operator", "Win a game on Normal difficulty"));
        achievements.Add(new Achievement("win_hard", "Elite Hacker", "Win a game on Hard difficulty"));
        achievements.Add(new Achievement("no_errors", "Flawless", "Complete a game without any errors"));
        achievements.Add(new Achievement("no_errors_hard", "Perfectionist", "Complete a game on Hard difficulty without errors"));

        achievements.Add(new Achievement("speed_runner", "Speed Runner", "Complete a game in under 5 minutes"));
        achievements.Add(new Achievement("lightning_fast", "Lightning Fast", "Complete a game in under 3 minutes"));

        achievements.Add(new Achievement("play_1h", "One-Hour Marathon", "Play for 1 hour without interruption"));
        achievements.Add(new Achievement("play_3h", "Maratonista", "Play for 3 hours without interruption"));
        achievements.Add(new Achievement("weekly_streak", "7-Day Streak", "Play the game on all 7 days of the week"));

        achievements.Add(new Achievement("save_master", "Save Master", "Use the save feature for the first time"));
        achievements.Add(new Achievement("movie_buff", "Movie Buff", "Watch the intro video"));

        achievements.Add(new Achievement("archivist", "The Archivist", "Create 10 save files"));

        achievements.Add(new Achievement("collector", "Collector", "Unlock 50% of achievements"));
        achievements.Add(new Achievement("all_achievements", "Please, No More", "Unlock all achievements"));
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

            CheckCollectorAchievement();
            CheckAllAchievements();
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

    public void CheckWeeklyStreak()
    {
        int dayIndex = (int)System.DateTime.Today.DayOfWeek;
        if (!daysPlayed[dayIndex])
        {
            daysPlayed[dayIndex] = true;
            SaveWeeklyStreak();
        }

        bool allDaysPlayed = true;
        foreach (bool played in daysPlayed)
        {
            if (!played)
            {
                allDaysPlayed = false;
                break;
            }
        }

        if (allDaysPlayed)
        {
            UnlockAchievement("weekly_streak");
        }
    }

    private void LoadWeeklyStreak()
    {
        if (PlayerPrefs.HasKey(WEEKLY_STREAK_KEY))
        {
            string data = PlayerPrefs.GetString(WEEKLY_STREAK_KEY);
            string[] parts = data.Split(',');
            for (int i = 0; i < 7; i++)
            {
                daysPlayed[i] = parts[i] == "1";
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
                daysPlayed[i] = false;
        }
    }

    private void SaveWeeklyStreak()
    {
        string data = "";
        for (int i = 0; i < 7; i++)
        {
            data += daysPlayed[i] ? "1" : "0";
            if (i < 6) data += ",";
        }
        PlayerPrefs.SetString(WEEKLY_STREAK_KEY, data);
        PlayerPrefs.Save();
    }

    public void CheckCollectorAchievement()
    {
        int unlockedCount = GetUnlockedCount();
        int totalCount = GetTotalCount();

        if (unlockedCount >= totalCount / 2)
        {
            UnlockAchievement("collector");
        }
    }

    public void CheckAllAchievements()
    {
        foreach (Achievement a in achievements)
        {
            if (!a.isUnlocked && a.id != "all_achievements")
                return;
        }
        UnlockAchievement("all_achievements");
    }
}