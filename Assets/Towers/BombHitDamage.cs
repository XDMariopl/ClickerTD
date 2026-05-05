using Effects;
using UnityEngine;

public class BombHitDamage : IHitEffect
{
    private int bombNth;
    private int bombDamage;
    public float bombRadius;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;
    private PlayerCursor cursor;
    private Sprite effectSprite;

    public BombHitDamage(int bombNth, int bombDamage, float bombRadius, TowerSFX sfx = null, PlayerCursor cursor = null, Sprite effectSprite = null)
    {
        this.bombNth = bombNth;
        this.bombDamage = bombDamage;
        this.bombRadius = bombRadius;
        this.towerSFX = sfx;
        this.cursor = cursor;
        this.effectSprite = effectSprite;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (localHitCounter % bombNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();

        Vector3 explosionCenter = context.target.transform.position;
        cursor?.TriggerEffectParticles(effectSprite, explosionCenter);

        DrawExplosionCircle(explosionCenter);

        foreach (var enemy in EnemyManager.ActiveEnemies)
        {
            float dist = Vector2.Distance(
                explosionCenter,
                enemy.transform.position
            );

            if (dist <= bombRadius)
                enemy.TakeDamage(bombDamage, EnemyDamageSource.TowerEffect);
        }
    }

    void DrawExplosionCircle(Vector3 center)
    {
        GameObject circle = new GameObject("BombExplosionCircle");

        LineRenderer lr = circle.AddComponent<LineRenderer>();

        int segments = 48;
        lr.positionCount = segments;
        lr.loop = true;

        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * bombRadius,
                Mathf.Sin(angle) * bombRadius,
                0
            );

            lr.SetPosition(i, center + pos);
        }

        Object.Destroy(circle, 0.25f);
    }
}
