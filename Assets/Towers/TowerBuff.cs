using UnityEngine;

public class TowerBuff : MonoBehaviour
{
    public float radius = 3f;
    public float damageMultiplier = 1.5f;

    private PlayerCursor cursor;
    private bool isActive;

    void Start()
    {
        cursor = FindFirstObjectByType<PlayerCursor>();
    }

    void Update()
    {
        float dist = Vector2.Distance(cursor.Position, transform.position);

        if (dist <= radius && !isActive)
        {
            cursor.SetMultiplier(damageMultiplier);
            isActive = true;
            Debug.Log("Buff ON");
        }
        else if (dist > radius && isActive)
        {
            cursor.SetMultiplier(1f);
            isActive = false;
            Debug.Log("Buff OFF");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
