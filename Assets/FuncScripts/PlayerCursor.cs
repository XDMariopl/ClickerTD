using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public float baseDamage = 1f;
    private float damageMultiplier = 1f;

    public Vector2 Position => transform.position;

    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }

    public int GetDamage()
    {
        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public void SetMultiplier(float value)
    {
        damageMultiplier = value;
    }
}
