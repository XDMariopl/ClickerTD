using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EnemyMover prefab;
        public int count;
        public EnemyModifierType modifiers;
    }

    public bool IsSpawning { get; private set; }

    public Path path;

    public float spawnDelay = 1f;

    public Transform enemyParent; // assign in Inspector (Canvas or container)

    public IEnumerator SpawnWave(RoundEntry round)
    {
        foreach (var entry in round.enemies)
        {
            yield return SpawnEnemyEntry(entry);
        }
    }

    public IEnumerator SpawnWave(IEnumerable<EnemyEntry> entries)
    {
        if (entries == null)
            yield break;

        foreach (var entry in entries)
            yield return SpawnEnemyEntry(entry);
    }

    IEnumerator SpawnEnemyEntry(EnemyEntry entry)
    {
        if (entry == null || entry.prefab == null || entry.count <= 0)
            yield break;

        for (int i = 0; i < entry.count; i++)
        {
            EnemyMover enemy = Instantiate(
                entry.prefab,
                enemyParent,
                false
            );

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.ApplyModifiers(entry.modifiers);

            enemy.Init(path);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
