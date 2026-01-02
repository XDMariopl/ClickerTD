using UnityEngine;
using Effects;

public class NthHitDamageEffect : IHitEffect
{
    private int everyN;
    private float multiplier;

    public NthHitDamageEffect(int everyN, float multiplier)
    {
        this.everyN = everyN;
        this.multiplier = multiplier;
    }

    public void OnHit(HitContext context)
    {
        if (context.hitCount % everyN == 0)
        {
            context.damage = Mathf.RoundToInt(context.damage * multiplier);
        }
    }
}
