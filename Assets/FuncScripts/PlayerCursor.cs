using System.Collections.Generic;
using UnityEngine;
using Effects;

public class PlayerCursor : MonoBehaviour
{
    public int baseDamage = 1;

    private int hitCounter = 0;
    private List<IHitEffect> effects = new();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        transform.position = pos;
    }

    public void RegisterEffect(IHitEffect effect)
    {
        effects.Add(effect);
        Debug.Log($"Effect registered: {effect.GetType().Name}");
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
        Debug.Log(hitCounter);

        foreach (var effect in effects)
            effect.OnHit(context);

        enemy.TakeDamage(context.damage);
    }
}
