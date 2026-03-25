using Effects;

public class ReverseHitEffect : IHitEffect
{
    private float reverseDuration;
    private int reverseNth;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;

    public ReverseHitEffect(float reverseDuration, int reverseNth, TowerSFX sfx = null)
    {
        this.reverseDuration = reverseDuration;
        this.reverseNth = reverseNth;
        this.towerSFX = sfx;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (reverseNth <= 0 || localHitCounter % reverseNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();

        EnemyMover mover = context.target.GetComponent<EnemyMover>();
        if (mover != null)
            mover.ApplyReverse(reverseDuration);
    }
}
