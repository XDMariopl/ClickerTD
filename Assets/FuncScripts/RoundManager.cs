using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    public EnemySpawner spawner;

    [Header("Rounds")]
    public RoundEntry[] rounds;
    public float delayBetweenRounds = 4f;

    [Header("Endless Mode")]
    public EndlessModeConfig endlessConfig;

    private int currentRound = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text roundText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(LevelManager.Instance != null && LevelManager.Instance.CurrentMode == LevelPlayMode.Endless
            ? EndlessLoop()
            : RoundLoop());
    }

    IEnumerator RoundLoop()
    {
        for (int i = 0; i < rounds.Length; i++)
        {
            currentRound = i + 1;
            UpdateUI();

            yield return StartCoroutine(
                spawner.SpawnWave(rounds[i])
            );

            yield return new WaitUntil(() =>
                EnemyManager.ActiveEnemies.Count == 0
            );

            yield return new WaitForSeconds(delayBetweenRounds);
        }

        Debug.Log("All rounds completed!");
        HandleWin();
    }

    IEnumerator EndlessLoop()
    {
        while (true)
        {
            currentRound++;
            RegisterEndlessMedalProgress();
            UpdateUI();

            List<EnemySpawner.EnemyEntry> wave = BuildEndlessWave(currentRound);
            yield return StartCoroutine(spawner.SpawnWave(wave));

            yield return new WaitUntil(() =>
                EnemyManager.ActiveEnemies.Count == 0
            );

            yield return new WaitForSeconds(GetEndlessDelayBetweenRounds());
        }
    }

    void UpdateUI()
    {
        if (roundText != null)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentMode == LevelPlayMode.Endless)
                roundText.text = "Endless " + currentRound;
            else
                roundText.text = currentRound + " / " + rounds.Length;
        }
    }

    void HandleWin()
    {
        int spent = MoneySystem.Instance != null ? MoneySystem.Instance.TotalSpent() : 0;
        int collected = MoneySystem.Instance != null ? MoneySystem.Instance.TotalCollected() : 0;
        int lostLives = PlayerHealth.Instance != null ? PlayerHealth.Instance.LostLives() : 0;

        LevelManager.Instance?.CompleteCurrentLevel();

        if (WinLoseUI.Instance != null)
            WinLoseUI.Instance.ShowWin(lostLives, spent, collected);
    }

    public int CurrentRound()
    {
        return currentRound;
    }

    List<EnemySpawner.EnemyEntry> BuildEndlessWave(int round)
    {
        List<EndlessEnemyOption> available = GetAvailableEndlessEnemies(round);
        List<EnemySpawner.EnemyEntry> generated = new List<EnemySpawner.EnemyEntry>();

        if (available.Count == 0)
            return generated;

        int budget = GetEndlessBudget(round);
        int safety = 0;

        while (budget > 0 && safety < 512)
        {
            safety++;

            List<EndlessEnemyOption> affordable = available.FindAll(option =>
                option != null &&
                option.prefab != null &&
                option.enemyCost > 0 &&
                option.enemyCost <= budget
            );

            if (affordable.Count == 0)
                break;

            EndlessEnemyOption selected = PickWeightedEnemy(affordable);
            int remainingBudget = budget - selected.enemyCost;
            EndlessEnemyModifierOption modifierOption = PickModifierOption(round, remainingBudget);
            int modifierCost = modifierOption != null ? Mathf.Max(0, modifierOption.extraCost) : 0;
            budget -= selected.enemyCost + modifierCost;

            generated.Add(new EnemySpawner.EnemyEntry
            {
                prefab = selected.prefab,
                count = 1,
                modifiers = modifierOption != null ? modifierOption.modifiers : EnemyModifierType.None
            });
        }

        ShuffleEntries(generated);
        return generated;
    }

    List<EndlessEnemyOption> GetAvailableEndlessEnemies(int round)
    {
        List<EndlessEnemyOption> available = new List<EndlessEnemyOption>();

        if (endlessConfig == null || endlessConfig.enemyOptions == null)
            return available;

        foreach (EndlessEnemyOption option in endlessConfig.enemyOptions)
        {
            if (option == null || option.prefab == null)
                continue;

            if (round < Mathf.Max(1, option.unlockRound))
                continue;

            if (option.lastRound > 0 && round > option.lastRound)
                continue;

            available.Add(option);
        }

        return available;
    }

    EndlessEnemyOption PickWeightedEnemy(List<EndlessEnemyOption> options)
    {
        int totalWeight = 0;

        foreach (EndlessEnemyOption option in options)
            totalWeight += Mathf.Max(1, option.weight);

        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach (EndlessEnemyOption option in options)
        {
            roll -= Mathf.Max(1, option.weight);
            if (roll < 0)
                return option;
        }

        return options[options.Count - 1];
    }

    EndlessEnemyModifierOption PickModifierOption(int round, int remainingBudget)
    {
        if (endlessConfig == null || endlessConfig.modifierOptions == null || endlessConfig.modifierOptions.Length == 0)
            return null;

        List<EndlessEnemyModifierOption> available = new List<EndlessEnemyModifierOption>();

        foreach (EndlessEnemyModifierOption option in endlessConfig.modifierOptions)
        {
            if (option == null)
                continue;

            if (option.modifiers == EnemyModifierType.None)
                continue;

            if (round < Mathf.Max(1, option.unlockRound))
                continue;

            if (Mathf.Max(0, option.extraCost) > remainingBudget)
                continue;

            available.Add(option);
        }

        int noModifierWeight = 1;
        int totalWeight = noModifierWeight;

        foreach (EndlessEnemyModifierOption option in available)
            totalWeight += Mathf.Max(1, option.weight);

        int roll = UnityEngine.Random.Range(0, totalWeight);
        if (roll < noModifierWeight)
            return null;

        roll -= noModifierWeight;

        foreach (EndlessEnemyModifierOption option in available)
        {
            roll -= Mathf.Max(1, option.weight);
            if (roll < 0)
                return option;
        }

        return null;
    }

    int GetEndlessBudget(int round)
    {
        int rampBonus = 0;

        if (endlessConfig != null && endlessConfig.budgetRampEvery > 0)
            rampBonus = ((round - 1) / endlessConfig.budgetRampEvery) * endlessConfig.extraRampAmount;

        if (endlessConfig == null)
            return 0;

        return endlessConfig.startingBudget + ((round - 1) * endlessConfig.budgetIncreasePerRound) + rampBonus;
    }

    void ShuffleEntries(List<EnemySpawner.EnemyEntry> entries)
    {
        for (int i = entries.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            EnemySpawner.EnemyEntry temp = entries[i];
            entries[i] = entries[swapIndex];
            entries[swapIndex] = temp;
        }
    }

    float GetEndlessDelayBetweenRounds()
    {
        if (endlessConfig == null)
            return delayBetweenRounds;

        return endlessConfig.delayBetweenRounds;
    }

    void RegisterEndlessMedalProgress()
    {
        if (LevelManager.Instance == null || LevelManager.Instance.CurrentMode != LevelPlayMode.Endless)
            return;

        string levelId = LevelManager.Instance.GetCurrentSceneLevelId();
        if (string.IsNullOrEmpty(levelId))
            return;

        ProgressManager.Instance?.RegisterEndlessRoundReached(levelId, currentRound);
    }
}
