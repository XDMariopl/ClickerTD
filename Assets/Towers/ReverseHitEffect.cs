using Effects;
using UnityEngine;

public class ReverseHitEffect : IHitEffect
{
    private float reverseDuration;
    private int reverseNth;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;
    private PlayerCursor cursor;
    private Sprite effectSprite;

    public ReverseHitEffect(float reverseDuration, int reverseNth, TowerSFX sfx = null, PlayerCursor cursor = null, Sprite effectSprite = null)
    {
        this.reverseDuration = reverseDuration;
        this.reverseNth = reverseNth;
        this.towerSFX = sfx;
        this.cursor = cursor;
        this.effectSprite = effectSprite;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (reverseNth <= 0 || localHitCounter % reverseNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();
        cursor?.TriggerEffectParticles(effectSprite, context.target.transform.position);

        EnemyMover mover = context.target.GetComponent<EnemyMover>();
        if (mover != null)
            mover.ApplyReverse(reverseDuration);
    }
}
