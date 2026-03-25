using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP= 5;
    private int currentHP;
    public int damageToPlayer = 1;
    public int moneyDrop = 5;
    private int moneyMultiplier = 1;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("Damage taken: "+dmg);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (MoneySystem.Instance != null)
            MoneySystem.Instance.AddMoney(moneyDrop * moneyMultiplier);

        Destroy(gameObject);
    }

    public void ApplyMoneyMultiplier(int multiplier)
    {
        if (multiplier <= 1) return;
        moneyMultiplier = Mathf.Max(moneyMultiplier, multiplier);
    }

    void OnEnable()
    {
        EnemyManager.ActiveEnemies.Add(this);
    }

    void OnDisable()
    {
        EnemyManager.ActiveEnemies.Remove(this);
    }
}
