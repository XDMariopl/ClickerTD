using UnityEngine;

[System.Serializable]
public class TowerLevel
{
    public TowerEffectType effectType;

    [Header("Upgrade")]
    public int upgradeCost;
    public Sprite towerBaseSprite;
    public Sprite towerEffectSprite;

    [Header("Nth Hit Damage")]
    public int everyN;
    public float multiplier;

    [Header("Chain Damage")]
    public int chainNth;
    public int chainHits;
    public int chainDamage;
    public float chainRadius;

    [Header("Bomb Damage")]
    public int bombNth;
    public int bombDamage;
    public float bombRadius;

    [Header("Plus Damage")]
    public int addDamage;
    public float multiplyDamage;
    public float addMultiplyRadius;

    [Header("Slow Effect")]
    public float slowPower;
    public float slowRadius;
    public int slowNth;

    [Header("Money Effect")]
    public int moneyMultiply;
    public int moneyNth;

    [Header("Reverse Effect")]
    public float reverseDuration;
    public int reverseNth;

}
