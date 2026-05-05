using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerProgressData
{
    public List<string> completedLevelIds = new List<string>();
    public List<string> unlockedLevelIds = new List<string>();
    public List<string> unlockedTowerIds = new List<string>();
    public List<LevelMedalProgress> levelMedals = new List<LevelMedalProgress>();
}

[Serializable]
public class LevelMedalProgress
{
    public string levelId;
    public int medalTier;
    public int bestEndlessRound;
}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [SerializeField] private string saveFileName = "player_progress.json";
    [SerializeField] private TowerData[] allTowers;

    private PlayerProgressData progress = new PlayerProgressData();

    public string SaveFilePath => System.IO.Path.Combine(Application.persistentDataPath, saveFileName);

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (HasSaveFile())
            LoadProgress();
        else
            CreateNewSave();
    }

    public bool HasSaveFile()
    {
        return System.IO.File.Exists(SaveFilePath);
    }

    public void CreateNewSave()
    {
        progress = new PlayerProgressData();
        RefreshTowerUnlocks();
        SaveProgress();
    }

    public void LoadProgress()
    {
        if (!HasSaveFile())
        {
            CreateNewSave();
            return;
        }

        string json = System.IO.File.ReadAllText(SaveFilePath);
        progress = JsonUtility.FromJson<PlayerProgressData>(json);

        if (progress == null)
            progress = new PlayerProgressData();

        EnsureLists();
        RefreshTowerUnlocks();
        SaveProgress();
    }

    public void SaveProgress()
    {
        EnsureLists();
        string json = JsonUtility.ToJson(progress, true);
        System.IO.File.WriteAllText(SaveFilePath, json);
    }

    public int CompletedLevelCount()
    {
        EnsureLists();
        return progress.completedLevelIds.Count;
    }

    public bool IsLevelCompleted(string levelId)
    {
        return !string.IsNullOrEmpty(levelId) && progress.completedLevelIds.Contains(levelId);
    }

    public bool IsLevelUnlocked(string levelId)
    {
        return !string.IsNullOrEmpty(levelId) && progress.unlockedLevelIds.Contains(levelId);
    }

    public void UnlockLevel(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return;

        if (!progress.unlockedLevelIds.Contains(levelId))
        {
            progress.unlockedLevelIds.Add(levelId);
            SaveProgress();
        }
    }

    public void CompleteLevel(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return;

        if (!progress.completedLevelIds.Contains(levelId))
            progress.completedLevelIds.Add(levelId);

        UnlockLevel(levelId);
        AwardMedal(levelId, 1);
        RefreshTowerUnlocks();
        SaveProgress();
    }

    public int GetMedalTier(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return 0;

        LevelMedalProgress medalProgress = GetOrCreateLevelMedalProgress(levelId);
        return medalProgress.medalTier;
    }

    public int GetBestEndlessRound(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return 0;

        LevelMedalProgress medalProgress = GetOrCreateLevelMedalProgress(levelId);
        return medalProgress.bestEndlessRound;
    }

    public void RegisterEndlessRoundReached(string levelId, int round)
    {
        if (string.IsNullOrEmpty(levelId) || round <= 0)
            return;

        LevelMedalProgress medalProgress = GetOrCreateLevelMedalProgress(levelId);
        bool changed = false;

        if (round > medalProgress.bestEndlessRound)
        {
            medalProgress.bestEndlessRound = round;
            changed = true;
        }

        int medalTier = GetEndlessMedalTier(round);
        if (medalTier > medalProgress.medalTier)
        {
            medalProgress.medalTier = medalTier;
            changed = true;
        }

        if (changed)
            SaveProgress();
    }

    public void AwardMedal(string levelId, int medalTier)
    {
        if (string.IsNullOrEmpty(levelId) || medalTier <= 0)
            return;

        LevelMedalProgress medalProgress = GetOrCreateLevelMedalProgress(levelId);
        if (medalTier <= medalProgress.medalTier)
            return;

        medalProgress.medalTier = medalTier;
        SaveProgress();
    }

    public bool IsTowerUnlocked(TowerData tower)
    {
        if (tower == null)
            return false;

        string towerId = GetTowerId(tower);
        if (string.IsNullOrEmpty(towerId))
            return tower.unlockAfterCompletedLevels <= CompletedLevelCount();

        if (progress.unlockedTowerIds.Contains(towerId))
            return true;

        if (tower.unlockAfterCompletedLevels <= CompletedLevelCount())
        {
            UnlockTower(tower);
            return true;
        }

        return false;
    }

    public void UnlockTower(TowerData tower)
    {
        if (tower == null)
            return;

        string towerId = GetTowerId(tower);
        if (string.IsNullOrEmpty(towerId))
            return;

        if (!progress.unlockedTowerIds.Contains(towerId))
        {
            progress.unlockedTowerIds.Add(towerId);
            SaveProgress();
        }
    }

    public void RefreshTowerUnlocks()
    {
        EnsureLists();

        if (allTowers == null)
            return;

        int completedLevels = CompletedLevelCount();

        foreach (TowerData tower in allTowers)
        {
            if (tower == null)
                continue;

            if (tower.unlockAfterCompletedLevels <= completedLevels)
            {
                string towerId = GetTowerId(tower);
                if (!string.IsNullOrEmpty(towerId) && !progress.unlockedTowerIds.Contains(towerId))
                    progress.unlockedTowerIds.Add(towerId);
            }
        }
    }

    private string GetTowerId(TowerData tower)
    {
        if (tower == null)
            return string.Empty;

        if (!string.IsNullOrWhiteSpace(tower.towerId))
            return tower.towerId;

        return tower.name;
    }

    private void EnsureLists()
    {
        if (progress.completedLevelIds == null)
            progress.completedLevelIds = new List<string>();

        if (progress.unlockedLevelIds == null)
            progress.unlockedLevelIds = new List<string>();

        if (progress.unlockedTowerIds == null)
            progress.unlockedTowerIds = new List<string>();

        if (progress.levelMedals == null)
            progress.levelMedals = new List<LevelMedalProgress>();
    }

    private LevelMedalProgress GetOrCreateLevelMedalProgress(string levelId)
    {
        EnsureLists();

        for (int i = 0; i < progress.levelMedals.Count; i++)
        {
            if (progress.levelMedals[i] != null && progress.levelMedals[i].levelId == levelId)
                return progress.levelMedals[i];
        }

        LevelMedalProgress medalProgress = new LevelMedalProgress
        {
            levelId = levelId,
            medalTier = 0,
            bestEndlessRound = 0
        };

        progress.levelMedals.Add(medalProgress);
        return medalProgress;
    }

    private int GetEndlessMedalTier(int round)
    {
        if (round >= 80)
            return 4;

        if (round >= 45)
            return 3;

        if (round >= 25)
            return 2;

        return 0;
    }
}
