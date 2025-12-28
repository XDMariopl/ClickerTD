using UnityEngine;

public class DmgClick : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public void ApplyDamage(int dmg)
    {
        enemyHealth.TakeDamage(dmg);
    }
}
