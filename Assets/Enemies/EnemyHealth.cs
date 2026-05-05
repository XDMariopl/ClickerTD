using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP= 5;
    private int currentHP;
    public int damageToPlayer = 1;
    public int moneyDrop = 5;
    [Header("Modifier Colors")]
    [SerializeField] private Color doubleHealthColor = Color.red;
    [SerializeField] private Color secondHitsColor = Color.yellow;
    [SerializeField] private Color towerEffectOnlyColor = new Color(0.6f, 0.2f, 1f, 1f);
    [SerializeField] private float modifierColorSwitchInterval = 0.6f;

    private int moneyMultiplier = 1;
    private EnemyModifierType modifiers = EnemyModifierType.None;
    private int incomingHitAttempts = 0;
    private SpriteRenderer[] spriteRenderers;
    private Color[] baseColors;
    private Color[] modifierColors = System.Array.Empty<Color>();
    private float modifierColorTimer = 0f;
    private int modifierColorIndex = 0;
    private MaterialPropertyBlock colorBlock;

    void Awake()
    {
        CacheRenderers();
    }

    void Start()
    {
        currentHP = maxHP;
        ApplyModifierVisuals(true);
    }

    void Update()
    {
        if (modifierColors == null || modifierColors.Length == 0)
            return;

        if (modifierColors.Length > 1)
        {
            modifierColorTimer += Time.deltaTime;
            if (modifierColorTimer >= modifierColorSwitchInterval)
            {
                modifierColorTimer = 0f;
                modifierColorIndex = (modifierColorIndex + 1) % modifierColors.Length;
            }
        }

        ApplyColorToRenderers(modifierColors[modifierColorIndex]);
    }

    public void TakeDamage(int dmg)
    {
        TakeDamage(dmg, EnemyDamageSource.DirectHit);
    }

    public void TakeDamage(int dmg, EnemyDamageSource source)
    {
        if (dmg <= 0)
            return;

        if (HasModifier(EnemyModifierType.OnlyTakesDamageFromTowerEffects) && source != EnemyDamageSource.TowerEffect)
            return;

        incomingHitAttempts++;

        if (HasModifier(EnemyModifierType.OnlySecondHitsDealDamage) && incomingHitAttempts % 2 != 0)
            return;

        currentHP -= dmg;
        Debug.Log("Damage taken: "+dmg);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (MoneySystem.Instance != null)
            MoneySystem.Instance.AddMoney(moneyDrop * moneyMultiplier);

        Destroy(gameObject);
    }

    public void ApplyMoneyMultiplier(int multiplier)
    {
        if (multiplier <= 1) return;
        moneyMultiplier = Mathf.Max(moneyMultiplier, multiplier);
    }

    public void ApplyModifiers(EnemyModifierType modifiers)
    {
        this.modifiers = modifiers;

        if (HasModifier(EnemyModifierType.DoubleHealth))
            maxHP *= 2;

        currentHP = maxHP;
        BuildModifierColors();
        ApplyModifierVisuals(true);
    }

    public bool HasModifier(EnemyModifierType modifier)
    {
        return (modifiers & modifier) != 0;
    }

    void CacheRenderers()
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
            return;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        baseColors = new Color[spriteRenderers.Length];
        colorBlock = new MaterialPropertyBlock();

        for (int i = 0; i < spriteRenderers.Length; i++)
            baseColors[i] = spriteRenderers[i].color;
    }

    void BuildModifierColors()
    {
        System.Collections.Generic.List<Color> colors = new System.Collections.Generic.List<Color>();

        if (HasModifier(EnemyModifierType.DoubleHealth))
            colors.Add(doubleHealthColor);

        if (HasModifier(EnemyModifierType.OnlySecondHitsDealDamage))
            colors.Add(secondHitsColor);

        if (HasModifier(EnemyModifierType.OnlyTakesDamageFromTowerEffects))
            colors.Add(towerEffectOnlyColor);

        modifierColors = colors.ToArray();
        modifierColorIndex = 0;
        modifierColorTimer = 0f;
    }

    void ApplyModifierVisuals(bool resetCycle)
    {
        CacheRenderers();

        if (modifierColors == null || modifierColors.Length == 0)
        {
            RestoreBaseColors();
            return;
        }

        if (resetCycle)
        {
            modifierColorIndex = 0;
            modifierColorTimer = 0f;
        }

        ApplyColorToRenderers(modifierColors[modifierColorIndex]);
    }

    void ApplyColorToRenderers(Color color)
    {
        if (spriteRenderers == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            spriteRenderers[i].color = color;
            spriteRenderers[i].GetPropertyBlock(colorBlock);
            colorBlock.SetColor("_Color", color);
            colorBlock.SetColor("_BaseColor", color);
            spriteRenderers[i].SetPropertyBlock(colorBlock);
        }
    }

    void RestoreBaseColors()
    {
        if (spriteRenderers == null || baseColors == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            Color restoredColor = i < baseColors.Length ? baseColors[i] : Color.white;
            spriteRenderers[i].color = restoredColor;
            spriteRenderers[i].GetPropertyBlock(colorBlock);
            colorBlock.SetColor("_Color", restoredColor);
            colorBlock.SetColor("_BaseColor", restoredColor);
            spriteRenderers[i].SetPropertyBlock(colorBlock);
        }
    }

    void OnEnable()
    {
        EnemyManager.ActiveEnemies.Add(this);
    }

    void OnDisable()
    {
        EnemyManager.ActiveEnemies.Remove(this);
    }
}
