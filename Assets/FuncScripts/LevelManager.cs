using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class LevelEntry
{
    public string levelId;
    public string displayName;
    public string sceneName;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Levels")]
    public LevelEntry[] levels;

    [Header("Selection UI")]
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text levelStatusText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button loadButton;

    private int selectedLevelIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.CopyBindingsFrom(this);
            Instance.RefreshUI();
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        EnsureInitialLevelUnlock();
        ClampSelectedIndex();
        RefreshUI();
    }

    public void NextLevel()
    {
        if (levels == null || levels.Length == 0)
            return;

        selectedLevelIndex = Mathf.Min(selectedLevelIndex + 1, levels.Length - 1);
        RefreshUI();
    }

    public void PreviousLevel()
    {
        if (levels == null || levels.Length == 0)
            return;

        selectedLevelIndex = Mathf.Max(selectedLevelIndex - 1, 0);
        RefreshUI();
    }

    public void LoadSelectedLevel()
    {
        if (levels == null || levels.Length == 0)
            return;

        LevelEntry level = levels[selectedLevelIndex];
        if (level == null || string.IsNullOrWhiteSpace(level.sceneName))
            return;

        string levelId = GetLevelId(level);
        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsLevelUnlocked(levelId))
            return;

        Time.timeScale = 1f;
        SceneManager.LoadScene(level.sceneName);
    }

    public void CompleteCurrentLevel()
    {
        int levelIndex = FindLevelIndexByScene(SceneManager.GetActiveScene().name);
        if (levelIndex < 0)
            return;

        LevelEntry currentLevel = levels[levelIndex];
        string currentLevelId = GetLevelId(currentLevel);

        ProgressManager.Instance?.CompleteLevel(currentLevelId);

        int nextLevelIndex = levelIndex + 1;
        if (nextLevelIndex < levels.Length)
        {
            string nextLevelId = GetLevelId(levels[nextLevelIndex]);
            ProgressManager.Instance?.UnlockLevel(nextLevelId);
        }

        RefreshUI();
    }

    public bool TryLoadNextLevelFromCurrentScene()
    {
        int currentLevelIndex = FindLevelIndexByScene(SceneManager.GetActiveScene().name);
        int nextLevelIndex = currentLevelIndex + 1;

        if (currentLevelIndex < 0 || nextLevelIndex >= levels.Length)
            return false;

        string nextLevelId = GetLevelId(levels[nextLevelIndex]);
        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsLevelUnlocked(nextLevelId))
            return false;

        selectedLevelIndex = nextLevelIndex;
        LoadSelectedLevel();
        return true;
    }

    public void RefreshUI()
    {
        if (levels == null || levels.Length == 0)
            return;

        ClampSelectedIndex();

        LevelEntry level = levels[selectedLevelIndex];
        string levelId = GetLevelId(level);
        bool unlocked = ProgressManager.Instance == null || ProgressManager.Instance.IsLevelUnlocked(levelId);
        bool completed = ProgressManager.Instance != null && ProgressManager.Instance.IsLevelCompleted(levelId);

        if (levelNameText != null)
            levelNameText.text = string.IsNullOrWhiteSpace(level.displayName) ? level.sceneName : level.displayName;

        if (levelStatusText != null)
        {
            if (!unlocked)
                levelStatusText.text = "Locked";
            else if (completed)
                levelStatusText.text = "Completed";
            else
                levelStatusText.text = "Unlocked";
        }

        if (previousButton != null)
            previousButton.interactable = selectedLevelIndex > 0;

        if (nextButton != null)
            nextButton.interactable = selectedLevelIndex < levels.Length - 1;

        if (loadButton != null)
            loadButton.interactable = unlocked;
    }

    private void EnsureInitialLevelUnlock()
    {
        if (levels == null || levels.Length == 0 || ProgressManager.Instance == null)
            return;

        string firstLevelId = GetLevelId(levels[0]);
        if (!ProgressManager.Instance.IsLevelUnlocked(firstLevelId))
            ProgressManager.Instance.UnlockLevel(firstLevelId);

        ProgressManager.Instance.RefreshTowerUnlocks();
        ProgressManager.Instance.SaveProgress();
    }

    private void CopyBindingsFrom(LevelManager other)
    {
        if (other == null)
            return;

        levels = other.levels;
        levelNameText = other.levelNameText;
        levelStatusText = other.levelStatusText;
        previousButton = other.previousButton;
        nextButton = other.nextButton;
        loadButton = other.loadButton;
    }

    private int FindLevelIndexByScene(string sceneName)
    {
        if (levels == null)
            return -1;

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] != null && levels[i].sceneName == sceneName)
                return i;
        }

        return -1;
    }

    private string GetLevelId(LevelEntry level)
    {
        if (level == null)
            return string.Empty;

        if (!string.IsNullOrWhiteSpace(level.levelId))
            return level.levelId;

        return level.sceneName;
    }

    private void ClampSelectedIndex()
    {
        if (levels == null || levels.Length == 0)
        {
            selectedLevelIndex = 0;
            return;
        }

        selectedLevelIndex = Mathf.Clamp(selectedLevelIndex, 0, levels.Length - 1);
    }
}
