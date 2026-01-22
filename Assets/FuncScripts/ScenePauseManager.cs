using UnityEngine;

public class ScenePauseManager : MonoBehaviour
{
    public static ScenePauseManager Instance;

    public bool IsSoftPaused { get; private set; }
    public bool IsHardPaused { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    // ---------- HARD PAUSE ----------
    public void EnterHardPause()
    {
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
}
