using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private PlayerCursor cursor;

    void Start()
    {
        cursor = FindFirstObjectByType<PlayerCursor>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (!hit.collider) return;

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                cursor.HitEnemy(enemy);
            }
        }
    }
}
