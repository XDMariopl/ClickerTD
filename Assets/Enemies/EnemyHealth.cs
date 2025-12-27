using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP= 5;
    private int currentHP;

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
        Destroy(gameObject);
    }
}
