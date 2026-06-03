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
    public Sprite previewImage;
}

public enum LevelPlayMode
{
    Campaign,
    Endless
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
    [SerializeField] private Image levelPreviewImage;
    [SerializeField] private Image[] medalImages;

    [Header("Medal Colors")]
    [SerializeField] private Color activeMedalColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color inactiveMedalColor = new Color32(58, 58, 58, 120);

    [Header("Endless Mode")]
    [SerializeField] private Button endlessButton;
    [SerializeField] private TMP_Text endlessStatusText;

    private int selectedLevelIndex;
    public LevelPlayMode CurrentMode { get; private set; } = LevelPlayMode.Campaign;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.CopyBindingsFrom(this);
            Instance.BindCopiedButtons();
            Instance.EnsureInitialLevelUnlock();
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

        CurrentMode = LevelPlayMode.Campaign;
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
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

    public string GetCurrentSceneLevelId()
    {
        int levelIndex = FindLevelIndexByScene(SceneManager.GetActiveScene().name);
        if (levelIndex < 0)
            return string.Empty;

        return GetLevelId(levels[levelIndex]);
    }

    public bool TryLoadNextLevelFromCurrentScene()
    {
        if (CurrentMode == LevelPlayMode.Endless)
            return false;

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

        if (levelPreviewImage != null)
        {
            levelPreviewImage.sprite = level.previewImage;
            levelPreviewImage.enabled = level.previewImage != null;
        }

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

        bool endlessUnlocked = IsSelectedLevelEndlessUnlocked();

        if (endlessButton != null)
            endlessButton.interactable = endlessUnlocked && levels != null && levels.Length > 0;

        if (endlessStatusText != null)
            endlessStatusText.text = endlessUnlocked ? "Unlocked" : "Complete this map to unlock";

        RefreshMedalVisuals(levelId);
    }

    public void LoadEndlessMode()
    {
        if (!IsSelectedLevelEndlessUnlocked())
            return;

        if (levels == null || levels.Length == 0)
            return;

        LevelEntry level = levels[selectedLevelIndex];
        if (level == null || string.IsNullOrWhiteSpace(level.sceneName))
            return;

        string levelId = GetLevelId(level);
        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsLevelUnlocked(levelId))
            return;

        CurrentMode = LevelPlayMode.Endless;
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
        Time.timeScale = 1f;
        SceneManager.LoadScene(level.sceneName);
    }

    public bool IsSelectedLevelEndlessUnlocked()
    {
        if (levels == null || levels.Length == 0 || ProgressManager.Instance == null)
            return false;

        ClampSelectedIndex();
        string levelId = GetLevelId(levels[selectedLevelIndex]);
        return ProgressManager.Instance.IsLevelCompleted(levelId);
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
        levelPreviewImage = other.levelPreviewImage;
        endlessButton = other.endlessButton;
        endlessStatusText = other.endlessStatusText;
        medalImages = other.medalImages;
        activeMedalColor = other.activeMedalColor;
        inactiveMedalColor = other.inactiveMedalColor;
    }

    private void BindCopiedButtons()
    {
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousLevel);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLevel);

        if (loadButton != null)
            loadButton.onClick.AddListener(LoadSelectedLevel);

        if (endlessButton != null)
            endlessButton.onClick.AddListener(LoadEndlessMode);
    }

    private void RefreshMedalVisuals(string levelId)
    {
        if (medalImages == null || medalImages.Length == 0)
            return;

        int medalTier = ProgressManager.Instance != null ? ProgressManager.Instance.GetMedalTier(levelId) : 0;

        for (int i = 0; i < medalImages.Length; i++)
        {
            if (medalImages[i] == null)
                continue;

            medalImages[i].color = i < medalTier ? activeMedalColor : inactiveMedalColor;
        }
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
