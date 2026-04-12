using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public int maxHealth = 20;
    private int currentHealth;
    private bool isDead;

    [Header("UI")]
    [SerializeField] private TMP_Text healthText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player took damage: " + amount);

        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth;
        }
    }

    void GameOver()
    {
        isDead = true;
        Debug.Log("GAME OVER");
        int round = 0;
        if (RoundManager.Instance != null)
            round = RoundManager.Instance.CurrentRound();

        int spent = MoneySystem.Instance != null ? MoneySystem.Instance.TotalSpent() : 0;
        int collected = MoneySystem.Instance != null ? MoneySystem.Instance.TotalCollected() : 0;

        if (WinLoseUI.Instance != null)
            WinLoseUI.Instance.ShowLose(round, spent, collected);
    }

    public int CurrentHealth()
    {
        return currentHealth;
    }

    public int LostLives()
    {
        return Mathf.Max(0, maxHealth - currentHealth);
    }
}
