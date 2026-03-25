using Effects;
using UnityEngine;

public class SlowHitEffect : IHitEffect
{
    private float slowPercent;
    private float slowRadius;
    private int slowNth;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;

    public SlowHitEffect(float slowPercent, float slowRadius, int slowNth, TowerSFX sfx = null)
    {
        this.slowPercent = slowPercent;
        this.slowRadius = slowRadius;
        this.slowNth = slowNth;
        this.towerSFX = sfx;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (slowNth <= 0 || localHitCounter % slowNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();

        Vector3 center = context.target.transform.position;

        foreach (var enemy in EnemyManager.ActiveEnemies)
        {
            float dist = Vector2.Distance(center, enemy.transform.position);
            if (dist > slowRadius)
                continue;

            EnemyMover mover = enemy.GetComponent<EnemyMover>();
            if (mover != null)
                mover.ApplySlow(slowPercent);
        }
    }
}
