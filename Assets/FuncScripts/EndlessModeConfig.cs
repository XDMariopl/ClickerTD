using System;
using UnityEngine;

[Serializable]
public class EndlessEnemyOption
{
    public EnemyMover prefab;
    public int enemyCost = 1;
    public int unlockRound = 1;
    public int lastRound = 0;
    public int weight = 1;
}

[CreateAssetMenu(menuName = "TD/Endless Mode Config")]
public class EndlessModeConfig : ScriptableObject
{
    public int startingBudget = 15;
    public int budgetIncreasePerRound = 5;
    public int budgetRampEvery = 5;
    public int extraRampAmount = 10;
    public float delayBetweenRounds = 4f;
    public EndlessEnemyOption[] enemyOptions;
    public EndlessEnemyModifierOption[] modifierOptions;
}
