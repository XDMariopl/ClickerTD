using UnityEngine;
using UnityEngine.UI;

public class ScenePauseManager : MonoBehaviour
{
    public static ScenePauseManager Instance;

    public bool IsSoftPaused { get; private set; }
    public bool IsHardPaused { get; private set; }
    public bool IsPaused => IsSoftPaused || IsHardPaused;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ConfigurePauseOverlaySorting();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        // Soft pause input (ESC)
        if (!IsHardPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSoftPause();
        }
    }

    // ---------- SOFT PAUSE ----------
    public void ToggleSoftPause()
    {
        if (IsHardPaused) return;

        if (IsSoftPaused)
            ResumeSoftPause();
        else
            EnterSoftPause();
    }

    void EnterSoftPause()
    {
        ConfigurePauseOverlaySorting();
        IsSoftPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Soft Pause");
    }

    void ResumeSoftPause()
    {
        IsSoftPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Resume Soft Pause");
    }

    public void EnterSoftPauseExternal()
    {
        if (IsHardPaused || IsSoftPaused)
            return;

        EnterSoftPause();
    }

    public void ExitSoftPauseExternal()
    {
        if (IsHardPaused || !IsSoftPaused)
            return;

        ResumeSoftPause();
    }

    // ---------- HARD PAUSE ----------
    public void EnterHardPause()
    {
        ConfigurePauseOverlaySorting();
        IsHardPaused = true;
        IsSoftPaused = false;
        Time.timeScale = 0f;
        Debug.Log("Hard Pause");
    }

    public void ExitHardPause()
    {
        IsHardPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Exit Hard Pause");
    }

    public void ExitPauseForSceneLoad()
    {
        IsSoftPaused = false;
        IsHardPaused = false;
        Time.timeScale = 1f;
    }

    void ConfigurePauseOverlaySorting()
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform target in transforms)
        {
            if (target == null || !target.gameObject.scene.IsValid())
                continue;

            if (!IsPauseOverlay(target.name))
                continue;

            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas == null)
                canvas = target.gameObject.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 500;

            if (target.GetComponent<GraphicRaycaster>() == null)
                target.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    bool IsPauseOverlay(string objectName)
    {
        return objectName == "PausePanel" ||
               objectName == "WinPanel" ||
               objectName == "MainPause" ||
               objectName == "WinPause" ||
               objectName == "LostPause";
    }
}
