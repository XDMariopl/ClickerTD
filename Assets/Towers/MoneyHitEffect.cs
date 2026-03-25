using Effects;

public class MoneyHitEffect : IHitEffect
{
    private int moneyMultiplier;
    private int moneyNth;

    private int localHitCounter = 0;
    private TowerSFX towerSFX;

    public MoneyHitEffect(int moneyMultiplier, int moneyNth, TowerSFX sfx = null)
    {
        this.moneyMultiplier = moneyMultiplier;
        this.moneyNth = moneyNth;
        this.towerSFX = sfx;
    }

    public void OnHit(HitContext context)
    {
        localHitCounter++;

        if (moneyNth <= 0 || localHitCounter % moneyNth != 0)
            return;

        if (context.target == null)
            return;

        towerSFX?.PlayAbilitySFX();

        context.target.ApplyMoneyMultiplier(moneyMultiplier);
    }
}
