using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EnemyMover prefab;
        public int count;
    }

    public bool IsSpawning { get; private set; }

    public Path path;

    public float spawnDelay = 1f;

    public Transform enemyParent; // assign in Inspector (Canvas or container)

    public IEnumerator SpawnWave(RoundEntry round)
    {
        foreach (var entry in round.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                EnemyMover enemy = Instantiate(
                    entry.prefab,
                    enemyParent,
                    false
                );

                enemy.Init(path);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
