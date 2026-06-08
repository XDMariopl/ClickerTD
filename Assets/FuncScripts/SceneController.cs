using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Default Scenes")]
    [Tooltip("Scene loaded when starting a new game")]
    public string firstLevelScene;

    [Tooltip("Main menu scene name")]
    public string mainMenuScene;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (Instance == this)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (Instance == this)
            BindSceneButtons();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindSceneButtons();
    }

    // ---------- BASIC LOADS ----------

    public void LoadScene(string sceneName)
    {
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    public void LoadFirstLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadSelectedLevel();
            return;
        }

        LoadScene(firstLevelScene);
    }

    public void LoadNextScene()
    {
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();

        if (LevelManager.Instance != null && LevelManager.Instance.TryLoadNextLevelFromCurrentScene())
            return;

        Time.timeScale = 1f;
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    // ---------- HARD PAUSE FRIENDLY ----------

    public void LoadNextSceneFromWin()
    {
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
        LoadNextScene();
    }

    public void RetryLevel()
    {
        ScenePauseManager.Instance?.ExitPauseForSceneLoad();
        ReloadCurrentScene();
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    private void BindSceneButtons()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttons)
        {
            if (button == null || !button.gameObject.scene.IsValid())
                continue;

            if (!IsUnderNamedParent(button.transform, "QuitPanel"))
                continue;

            if (button.name != "Yes")
                continue;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(QuitGame);
        }
    }

    private bool IsUnderNamedParent(Transform transformToCheck, string parentName)
    {
        Transform current = transformToCheck;
        while (current != null)
        {
            if (current.name == parentName)
                return true;

            current = current.parent;
        }

        return false;
    }
}
