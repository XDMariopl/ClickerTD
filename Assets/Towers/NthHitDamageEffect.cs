using UnityEngine;
using Effects;

public class NthHitDamageEffect : IHitEffect
{
    private int everyN;
    private float multiplier;
    private int localHitCounter = 0;
    private TowerSFX towerSFX;
    private PlayerCursor cursor;
    private Sprite effectSprite;

    // Pass TowerSFX when creating the effect
    public NthHitDamageEffect(int everyN, float multiplier, TowerSFX sfx = null, PlayerCursor cursor = null, Sprite effectSprite = null)
    {
        this.everyN = everyN;
        this.multiplier = multiplier;
        this.towerSFX = sfx;
        this.cursor = cursor;
        this.effectSprite = effectSprite;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (localHitCounter % everyN == 0)
        {
            context.damage = Mathf.RoundToInt(context.damage * multiplier);
            context.damageSource = EnemyDamageSource.TowerEffect;

            // Play SFX
            towerSFX?.PlayAbilitySFX();
            if (context.target != null)
                cursor?.TriggerEffectParticles(effectSprite, context.target.transform.position);

            Debug.Log($"NthHit triggered ({localHitCounter})");
        }
    }
}
