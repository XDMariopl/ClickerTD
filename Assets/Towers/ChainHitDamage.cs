using UnityEngine;
using System.Collections.Generic;
using Effects;

public class ChainHitDamage : IHitEffect
{
    private int chainNth;
    private int chainHits;
    private int chainDamage;
    private float chainRadius;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;

    public ChainHitDamage(int chainNth, int chainHits, int chainDamage, float chainRadius, TowerSFX sfx = null)
    {
        this.chainNth = chainNth;
        this.chainHits = chainHits;
        this.chainDamage = chainDamage;
        this.chainRadius = chainRadius;
        this.towerSFX = sfx;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (localHitCounter % chainNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();

        Chain(context.target);
    }

    void DrawChainLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("ChainLine");
        ChainLineVisual visual = lineObj.AddComponent<ChainLineVisual>();
        visual.Init(start, end);
    }


    void Chain(EnemyHealth startEnemy)
    {
        HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();
        EnemyHealth current = startEnemy;

        hitEnemies.Add(current);

        for (int i = 0; i < chainHits; i++)
        {
            EnemyHealth next = FindClosestEnemy(current, hitEnemies);

            if (next == null)
                break;

            next.TakeDamage(chainDamage);

            DrawChainLine(current.transform.position, next.transform.position);

            hitEnemies.Add(next);
            current = next;
        }
    }


    EnemyHealth FindClosestEnemy(EnemyHealth from, HashSet<EnemyHealth> ignore)
    {
        EnemyHealth[] allEnemies = Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        float closestDist = Mathf.Infinity;
        EnemyHealth closest = null;

        foreach (var enemy in allEnemies)
        {
            if (ignore.Contains(enemy))
                continue;

            float dist = Vector2.Distance(
                from.transform.position,
                enemy.transform.position
            );

            if (dist <= chainRadius && dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
