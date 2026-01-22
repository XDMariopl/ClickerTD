using UnityEngine;
using UnityEngine.SceneManagement;

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

    // ---------- BASIC LOADS ----------

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    public void LoadFirstLevel()
    {
        LoadScene(firstLevelScene);
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1f;
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);
    }

    // ---------- HARD PAUSE FRIENDLY ----------

    public void LoadNextSceneFromWin()
    {
        ScenePauseManager.Instance.ExitHardPause();
        LoadNextScene();
    }

    public void RetryLevel()
    {
        ScenePauseManager.Instance.ExitHardPause();
        ReloadCurrentScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
