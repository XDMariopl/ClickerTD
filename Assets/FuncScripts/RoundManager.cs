using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    public EnemySpawner spawner;

    [Header("Rounds")]
    public RoundEntry[] rounds;
    public float delayBetweenRounds = 4f;

    private int currentRound = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text roundText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RoundLoop());
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

    void UpdateUI()
    {
        if (roundText != null)
            roundText.text = currentRound + " / " + rounds.Length;
    }

    void HandleWin()
    {
        int spent = MoneySystem.Instance != null ? MoneySystem.Instance.TotalSpent() : 0;
        int collected = MoneySystem.Instance != null ? MoneySystem.Instance.TotalCollected() : 0;
        int lostLives = PlayerHealth.Instance != null ? PlayerHealth.Instance.LostLives() : 0;

        if (WinLoseUI.Instance != null)
            WinLoseUI.Instance.ShowWin(lostLives, spent, collected);
    }

    public int CurrentRound()
    {
        return currentRound;
    }
}
