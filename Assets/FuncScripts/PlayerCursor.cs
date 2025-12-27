using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public float baseDamage = 1f;
    private float damageMultiplier = 1f;

    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        transform.position = pos;
    }

    public int GetDamage()
    {
        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public void AddMultiplier(float value)
    {
        damageMultiplier *= value;
    }

    public void RemoveMultiplier(float value)
    {
        damageMultiplier /= value;
    }
}
