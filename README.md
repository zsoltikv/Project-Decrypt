# 🔓 Decrypt - Hacking Puzzle Game

<div align="center">

[![Tech Stack](https://skillicons.dev/icons?i=unity,visualstudio,github,cs)](https://skillicons.dev)
![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20PC-green?style=flat-square)
![Repo Size](https://img.shields.io/github/repo-size/zsoltikv/Project-Decrypt?style=flat-square)
![Last Commit](https://img.shields.io/github/last-commit/zsoltikv/Project-Decrypt?style=flat-square)
![Commits](https://img.shields.io/github/commit-activity/m/zsoltikv/Project-Decrypt?style=flat-square)

**A hacking simulator with 10+ mini-games, achievements, and speedrun mechanics**

</div>

## 🎮 Overview

Break into a secure system by completing puzzle mini-games. Each game reveals part of the password. Race against time across three difficulty levels.

## ✨ Features

- 🧩 **9 Mini-Games** - Memory, rhythm, logic puzzles
- 🏆 **25+ Achievements** - Skill-based and collection rewards
- ⏱️ **Speedrun Mode** - Time tracking and leaderboards
- 🎯 **3 Difficulties** - Easy, Normal, Hard
- 💾 **Save System** - JSON-based progress tracking
- 🎵 **Dynamic Audio** - Scene-persistent music system

## 🕹️ Mini-Games

| Game | Type | Description |
|------|------|-------------|
| **Byte Sorter** | Memory | Match binary number pairs in 4x3 grid      |
| **Cable Manager** | Logic | Connect color-coded ports via drag-drop   |
| **Hex Puzzle** | Typing | Enter hex sequences, multi-round            |
| **Lights On** | Puzzle | Toggle switches with cascade effects         |
| **Rhythm Decode** | Reflex | Press correct buttons before timeout     |
| **Sequence Hack** | Memory | Simon-says with increasing sequences     |
| **Signal Stabilize** | Skill | Keep dot inside moving target zone     |
| **Sine Wave Scanner** | Matching | Adjust amplitude/frequency sliders |
| **Malware Defense** | Action | Click to destroy incoming viruses      |
| **Password Terminal** | Final | Enter decrypted password to win       |

## 🏗️ Architecture

### Singleton Managers
```csharp
GameSettingsManager.Instance   // Difficulty, errors, password
AchievementManager.Instance    // 25+ achievements, JSON save
AudioManager.Instance          // Music control, scene persistence
TimerScript.Instance           // Global playtime tracking
```

### Scene Flow
```
IntroScene → MenuScene → FirstCutscene → GameScene (Hub)
   ↓
[9 Mini-Game Scenes]
   ↓
PasswordScene → OutroCutscene → SecretFileScene
```

## 📦 Installation
```bash
git clone https://github.com/yourusername/decrypt-game.git
```

**Requirements:** Unity 2021.3+, TextMeshPro, Input System

**Build Settings:** Add all scenes in order, set target platform (Android/PC)

## 🎯 Difficulty Scaling

| Parameter | Easy | Normal | Hard |
|-----------|------|--------|------|
| Byte Sorter Time | 40s | 30s | 20s |
| Hex Rounds | 3 | 5 | 7 |
| Malware HP | 100 | 60 | 40 |
| Sequence Rounds | 4 | 6 | 8 |

## 🏆 Key Achievements

- **First Steps** - Complete first mini-game
- **Completionist** - Finish all games in one run
- **Flawless** - Zero errors on any difficulty
- **Lightning Fast** - Complete under 3 minutes
- **Weekly Streak** - Play 7 consecutive days

## 💾 Save System

**Location:** `PlayerPrefs` + JSON files in persistent data path

**Data:**
- Leaderboard: player name, time, errors, difficulty
- Achievements: unlocked IDs array
- Settings: difficulty, audio preferences

## 🔧 Key Scripts

| Script | Purpose |
|--------|---------|
| `AchievementManager.cs` | Tracks all achievements, weekly streaks |
| `GameSettingsManager.cs` | Global state, difficulty, password gen |
| `AudioManager.cs` | Music shuffling, scene transitions |
| `CanvasScript.cs` | UI panel fade animations |
| `TimerScript.cs` | Playtime tracking for speedruns |

## 🎨 Asset Structure
```
Assets/
├── Scenes/          # All game scenes
├── Scripts/         # C# logic
│   ├── Managers/    # Singletons
│   └── MiniGames/   # Game-specific
├── Resources/
│   └── AppIcons/    # Mini-game icons
└── Prefabs/         # Reusable UI
```

## 🤝 Contributing

1. Fork the repo
2. Create feature branch (`git checkout -b feature/amazing`)
3. Commit changes (`git commit -m 'Add feature'`)
4. Push to branch (`git push origin feature/amazing`)
5. Open Pull Request

---

<div align="center">
Made with ❤️ for PENdroid using Unity
</div>
