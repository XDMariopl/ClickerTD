using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectButton : MonoBehaviour
{
    [SerializeField] private TowerData tower;
    [SerializeField] private TowerPlace towerPlace;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private TMP_Text lockedText;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    void Start()
    {
        RefreshState();
    }

    void OnEnable()
    {
        RefreshState();
    }

    public void SelectTower()
    {
        if (tower == null || towerPlace == null)
            return;

        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsTowerUnlocked(tower))
            return;

        towerPlace.StartPlacing(tower);
    }

    public void RefreshState()
    {
        bool unlocked = ProgressManager.Instance == null || ProgressManager.Instance.IsTowerUnlocked(tower);

        if (button != null)
            button.interactable = unlocked;

        if (lockedVisual != null)
            lockedVisual.SetActive(!unlocked);

        if (lockedText != null && tower != null)
            lockedText.text = unlocked ? string.Empty : "Unlocks after level " + tower.unlockAfterCompletedLevels;
    }
}
