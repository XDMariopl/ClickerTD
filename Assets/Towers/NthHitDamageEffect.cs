using UnityEngine;
using Effects;

public class NthHitDamageEffect : IHitEffect
{
    private int everyN;
    private float multiplier;
    private int localHitCounter = 0;
    private TowerSFX towerSFX;

    // Pass TowerSFX when creating the effect
    public NthHitDamageEffect(int everyN, float multiplier, TowerSFX sfx = null)
    {
        this.everyN = everyN;
        this.multiplier = multiplier;
        this.towerSFX = sfx;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (localHitCounter % everyN == 0)
        {
            context.damage = Mathf.RoundToInt(context.damage * multiplier);

            // Play SFX
            towerSFX?.PlayAbilitySFX();

            Debug.Log($"NthHit triggered ({localHitCounter})");
        }
    }
}
