using UnityEngine;
using Effects;

public class TowerBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float radius = 2.5f;
    public TowerLevel[] levels;

    [Header("Visual")]
    public Color radiusColor = new Color(0f, 1f, 0f, 0.6f);
    public float lineWidth = 0.05f;
    public int segments = 64;

    public int currentLevel = 0;
    private PlayerCursor cursor;
    private IHitEffect activeEffect;
    private bool active;

    public bool isPreview = false;

    private LineRenderer circle;

    void Start()
    {
        cursor = FindFirstObjectByType<PlayerCursor>();
        CreateRadiusVisual();
        circle.enabled = false;
    }

    void Update()
    {
        if (isPreview) return;

        float dist = Vector2.Distance(cursor.transform.position, transform.position);

        if (dist <= radius && !active)
            Activate();
        else if (dist > radius && active)
            Deactivate();

        circle.enabled = active;
    }


    void Activate()
    {
        if (active) return;

        TowerLevel lvl = levels[currentLevel];

        activeEffect = CreateEffect(lvl);
        cursor.RegisterEffect(activeEffect);

        active = true;
    }


    void Deactivate()
    {
        if (!active) return;

        cursor.UnregisterEffect(activeEffect);
        activeEffect = null;

        active = false;
    }

    void OnDestroy()
    {
        if (active && cursor != null && activeEffect != null)
        {
            cursor.UnregisterEffect(activeEffect);
            activeEffect = null;
            active = false;
        }
    }

    public bool CanUpgrade()
    {
        return levels != null && currentLevel < levels.Length - 1;
    }

    public int NextUpgradeCost()
    {
        if (!CanUpgrade())
            return 0;

        return levels[currentLevel + 1].upgradeCost;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade())
            return false;

        int cost = levels[currentLevel + 1].upgradeCost;

        if (MoneySystem.Instance == null || !MoneySystem.Instance.SpendMoney(cost))
            return false;

        currentLevel++;

        if (active && cursor != null)
        {
            cursor.UnregisterEffect(activeEffect);
            activeEffect = CreateEffect(levels[currentLevel]);
            cursor.RegisterEffect(activeEffect);
        }

        return true;
    }


    IHitEffect CreateEffect(TowerLevel lvl)
    {
        TowerSFX sfx = GetComponent<TowerSFX>(); // optional

        switch (lvl.effectType)
        {
            case TowerEffectType.NthHitDamage:
                return new NthHitDamageEffect(lvl.everyN, lvl.multiplier, sfx);

            case TowerEffectType.ChainDamage:
                return new ChainHitDamage(
                    lvl.chainNth,
                    lvl.chainHits,
                    lvl.chainDamage,
                    lvl.chainRadius,
                    sfx
                );
            case TowerEffectType.BombDamage:
                return new BombHitDamage(lvl.bombNth, lvl.bombDamage, lvl.bombRadius, sfx);

            case TowerEffectType.SlowEffect:
                return new SlowHitEffect(lvl.slowPower, lvl.slowRadius, lvl.slowNth, sfx);

            case TowerEffectType.MoneyEffect:
                return new MoneyHitEffect(lvl.moneyMultiply, lvl.moneyNth, sfx);

            case TowerEffectType.ReverseEffect:
                return new ReverseHitEffect(lvl.reverseDuration, lvl.reverseNth, sfx);


        }

        return null;
    }


    // ---------- RADIUS VISUAL ----------

    void CreateRadiusVisual()
    {
        GameObject go = new GameObject("RadiusVisual");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        circle = go.AddComponent<LineRenderer>();
        circle.useWorldSpace = false;
        circle.loop = true;
        circle.positionCount = segments;
        circle.startWidth = lineWidth;
        circle.endWidth = lineWidth;

        circle.material = new Material(Shader.Find("Sprites/Default"));
        circle.startColor = radiusColor;
        circle.endColor = radiusColor;

        DrawCircle();
    }

    void DrawCircle()
    {
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );
            circle.SetPosition(i, pos);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
