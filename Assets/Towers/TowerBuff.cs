using UnityEngine;

public class TowerBuff : MonoBehaviour
{
    public float damageMultiplier = 2f;

    private CircleCollider2D radiusCollider;
    private SpriteRenderer radiusVisual;

    void Awake()
    {
        radiusCollider = GetComponent<CircleCollider2D>();
        radiusVisual = GetComponentInChildren<SpriteRenderer>();

        // Scale visual to match collider exactly
        float diameter = radiusCollider.radius * 2f;
        radiusVisual.transform.localScale = new Vector3(diameter, diameter, 1f);

        radiusVisual.enabled = false;
    }

    void OnMouseEnter()
    {
        radiusVisual.enabled = true;
    }

    void OnMouseExit()
    {
        radiusVisual.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCursor cursor = other.GetComponent<PlayerCursor>();
        if (cursor != null)
        {
            cursor.AddMultiplier(damageMultiplier);
            Debug.Log("dmg buff on");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerCursor cursor = other.GetComponent<PlayerCursor>();
        if (cursor != null)
        {
            cursor.RemoveMultiplier(damageMultiplier);
            Debug.Log("dmg buff off");
        }
    }
}
