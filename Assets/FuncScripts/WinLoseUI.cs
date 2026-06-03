using TMPro;
using UnityEngine;

public class WinLoseUI : MonoBehaviour
{
    public static WinLoseUI Instance;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Lose UI")]
    public TMP_Text loseText1;
    public TMP_Text loseText2;

    [Header("Win UI")]
    public TMP_Text winText1;
    public TMP_Text winText2;

    void Awake()
    {
        Instance = this;

        if (winPanel != null)
            winPanel.SetActive(false);
        if (losePanel != null)
            losePanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ShowLose(int losingRound, int spent, int collected)
    {
        if (losePanel != null)
            losePanel.SetActive(true);
        if (winPanel != null)
            winPanel.SetActive(false);

        if (loseText1 != null)
            loseText1.text = $"Losing round: {losingRound}";

        if (loseText2 != null)
            loseText2.text = $"Spent: {spent}\nCollected: {collected}";

        ScenePauseManager.Instance?.EnterSoftPauseExternal();
    }

    public void ShowWin(int lostLives, int spent, int collected)
    {
        if (winPanel != null)
            winPanel.SetActive(true);
        if (losePanel != null)
            losePanel.SetActive(false);

        if (winText1 != null)
        {
            if (lostLives <= 0)
                winText1.text = "No lives were lost";
            else
                winText1.text = $"Lives lost: {lostLives}";
        }

        if (winText2 != null)
            winText2.text = $"Spent: {spent}\nCollected: {collected}";

        ScenePauseManager.Instance?.EnterSoftPauseExternal();
    }
}
