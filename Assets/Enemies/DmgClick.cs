using UnityEngine;

public class DmgClick : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private PlayerCursor playerCursor;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        playerCursor = FindFirstObjectByType<PlayerCursor>();
    }

    public void ApplyDamage(int dmg)
    {
        GetComponent<EnemyHealth>().TakeDamage(dmg);
    }
}
