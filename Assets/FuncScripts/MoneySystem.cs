using UnityEngine;
using TMPro;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem Instance;

    public int startMoney = 100;
    private int currentMoney;

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentMoney = startMoney;
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        return currentMoney >= amount;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        UpdateUI();
        return true;
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = "$ " + currentMoney;
    }

    public int CurrentMoney()
    {
        return currentMoney;
    }
}