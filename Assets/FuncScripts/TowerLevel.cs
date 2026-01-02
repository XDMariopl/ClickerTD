using UnityEngine;

[System.Serializable]
public class TowerLevel
{
    public TowerEffectType effectType;

    [Header("Nth Hit Damage")]
    public int everyN;
    public float multiplier;

    [Header("Chain Damage")]
    public float chainRadius;
    public int chainDamage;
}
