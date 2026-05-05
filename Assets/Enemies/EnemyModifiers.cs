using System;

[Flags]
public enum EnemyModifierType
{
    None = 0,
    DoubleHealth = 1 << 0,
    OnlySecondHitsDealDamage = 1 << 1,
    OnlyTakesDamageFromTowerEffects = 1 << 2
}

public enum EnemyDamageSource
{
    DirectHit,
    TowerEffect
}

[Serializable]
public class EndlessEnemyModifierOption
{
    public EnemyModifierType modifiers = EnemyModifierType.None;
    public int extraCost = 0;
    public int unlockRound = 1;
    public int weight = 1;
}
