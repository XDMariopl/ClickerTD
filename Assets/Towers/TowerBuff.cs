using UnityEngine;
using Effects;

public class TowerBuff : MonoBehaviour
{
    public TowerLevel[] levels;
    public float radius = 3f;

    private int currentLevel = 0;
    private PlayerCursor cursor;
    private IHitEffect activeEffect;
    private bool active;

    void Start()
    {
        cursor = FindFirstObjectByType<PlayerCursor>();
    }

    void Update()
    {
        float dist = Vector2.Distance(cursor.transform.position, transform.position);

        if (dist <= radius && !active)
            Activate();
        else if (dist > radius && active)
            Deactivate();
    }

    void Activate()
    {
        TowerLevel lvl = levels[currentLevel];

        activeEffect = CreateEffect(lvl);
        cursor.RegisterEffect(activeEffect);
        active = true;
    }

    void Deactivate()
    {
        cursor.UnregisterEffect(activeEffect);
        activeEffect = null;
        active = false;
    }

    IHitEffect CreateEffect(TowerLevel lvl)
    {
        switch (lvl.effectType)
        {
            case TowerEffectType.NthHitDamage:
                return new NthHitDamageEffect(lvl.everyN, lvl.multiplier);

        }

        return null;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
