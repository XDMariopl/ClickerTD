using UnityEngine;

public class TowerSFX : MonoBehaviour
{
    [Header("Optional SFX for abilities")]
    public AudioClip abilityClip;
    public float volume = 1f;

    private AudioSource source;

    void Awake()
    {
        // If the tower already has an AudioSource, use it
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D sound
        source.volume = volume;
    }

    public void PlayAbilitySFX()
    {
        if (abilityClip != null)
            source.PlayOneShot(abilityClip, volume);
    }
}
