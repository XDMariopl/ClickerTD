using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP= 5;
    private int currentHP;
    public int damageToPlayer = 1;
    public int moneyDrop = 5;

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
            MoneySystem.Instance.AddMoney(moneyDrop);

        Destroy(gameObject);
    }
}
