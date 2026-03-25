using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

public class RoundManager : MonoBehaviour
{
    public EnemySpawner spawner;

    [Header("Rounds")]
    public RoundEntry[] rounds;
    public float delayBetweenRounds = 4f;

    private int currentRound = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text roundText;

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
    }

    void UpdateUI()
    {
        if (roundText != null)
            roundText.text = currentRound + " / " + rounds.Length;
    }
}