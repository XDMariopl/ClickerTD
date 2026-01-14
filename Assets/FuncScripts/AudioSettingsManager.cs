using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    void Start()
    {
        LoadVolumes();
    }

    // Slider values expected: 0.0001f > 1f
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", LinearToDb(value));
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        audioMixer.SetFloat("MasterVolume", LinearToDb(master));
        audioMixer.SetFloat("MusicVolume", LinearToDb(music));
        audioMixer.SetFloat("SFXVolume", LinearToDb(sfx));
    }

    float LinearToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
