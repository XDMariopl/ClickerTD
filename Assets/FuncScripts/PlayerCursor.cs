using System.Collections.Generic;
using UnityEngine;
using Effects;

public class PlayerCursor : MonoBehaviour
{
    public int baseDamage = 1;

    private int hitCounter = 0;
    private List<IHitEffect> effects = new();

    public void RegisterEffect(IHitEffect effect)
    {
        effects.Add(effect);
    }

    public void UnregisterEffect(IHitEffect effect)
    {
        effects.Remove(effect);
    }

    public void HitEnemy(EnemyHealth enemy)
    {
        hitCounter++;

        HitContext context = new HitContext
        {
            hitCount = hitCounter,
            damage = baseDamage,
            target = enemy
        };

        foreach (var effect in effects)
            effect.OnHit(context);

        enemy.TakeDamage(context.damage);
    }
}
