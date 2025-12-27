using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private PlayerCursor playerCursor;

    void Start()
    {
        playerCursor = FindFirstObjectByType<PlayerCursor>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider == null) return;

            DmgClick dmgClick = hit.collider.GetComponent<DmgClick>();
            if (dmgClick != null)
            {
                int dmg = playerCursor.GetDamage();
                dmgClick.ApplyDamage(dmg);
            }
        }
    }
}
