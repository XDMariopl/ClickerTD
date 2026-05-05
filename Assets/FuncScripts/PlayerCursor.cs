using System.Collections.Generic;
using UnityEngine;
using Effects;

public class PlayerCursor : MonoBehaviour
{
    public int baseDamage = 1;

    private List<IHitEffect> effects = new();
    [SerializeField] private ParticleSystem effectParticlePrefab;
    private Material runtimeParticleMaterial;

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
        HitContext context = new HitContext
        {
            damage = baseDamage,
            target = enemy,
            damageSource = EnemyDamageSource.DirectHit
        };

        foreach (var effect in effects)
            effect.OnHit(context);

        enemy.TakeDamage(context.damage, context.damageSource);
    }

    public void TriggerEffectParticles(Sprite effectSprite, Vector3 worldPosition)
    {
        if (effectParticlePrefab == null || effectSprite == null)
            return;

        ParticleSystem spawnedParticles = Instantiate(effectParticlePrefab, worldPosition, Quaternion.identity);
        ApplyParticleMaterial(spawnedParticles, effectSprite);
        ParticleSystem.TextureSheetAnimationModule textureSheet = spawnedParticles.textureSheetAnimation;

        while (textureSheet.spriteCount > 0)
            textureSheet.RemoveSprite(0);

        textureSheet.enabled = true;
        textureSheet.mode = ParticleSystemAnimationMode.Sprites;
        textureSheet.AddSprite(effectSprite);

        spawnedParticles.Clear();
        spawnedParticles.Play();

        float lifetime = spawnedParticles.main.duration;
        if (spawnedParticles.main.startLifetime.mode == ParticleSystemCurveMode.Constant)
            lifetime += spawnedParticles.main.startLifetime.constantMax;

        Destroy(spawnedParticles.gameObject, lifetime + 0.25f);
    }

    void ApplyParticleMaterial(ParticleSystem particleSystem, Sprite effectSprite)
    {
        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        if (renderer == null)
            return;

        if (runtimeParticleMaterial == null)
        {
            Shader shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader != null)
                runtimeParticleMaterial = new Material(shader);
        }

        if (runtimeParticleMaterial == null)
            return;

        runtimeParticleMaterial.mainTexture = effectSprite.texture;
        renderer.sharedMaterial = runtimeParticleMaterial;
    }

}
