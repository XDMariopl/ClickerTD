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

    public Path path;
    public EnemyEntry[] enemies;

    public float spawnDelay = 1f;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    public Transform enemyParent; // assign in Inspector (Canvas or container)

    IEnumerator SpawnWave()
    {
        foreach (var entry in enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                EnemyMover enemy = Instantiate(
                    entry.prefab,
                    enemyParent,
                    false // IMPORTANT
                );

                enemy.Init(path);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
