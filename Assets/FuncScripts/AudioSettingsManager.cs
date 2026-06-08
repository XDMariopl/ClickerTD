using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        LoadVolumes();
        BindVolumeSliders();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadVolumes();
        BindVolumeSliders();
    }

    // Slider values expected: 0f to 1f.
    public void SetMasterVolume(float value)
    {
        SetVolume(MASTER_KEY, "MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume(MUSIC_KEY, "MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume(SFX_KEY, "SFXVolume", value);
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

    void BindVolumeSliders()
    {
        FindVolumeSliders();

        BindSlider(masterSlider, PlayerPrefs.GetFloat(MASTER_KEY, 1f), SetMasterVolume);
        BindSlider(musicSlider, PlayerPrefs.GetFloat(MUSIC_KEY, 1f), SetMusicVolume);
        BindSlider(sfxSlider, PlayerPrefs.GetFloat(SFX_KEY, 1f), SetSFXVolume);
    }

    void FindVolumeSliders()
    {
        masterSlider = null;
        musicSlider = null;
        sfxSlider = null;

        Slider[] sliders = Resources.FindObjectsOfTypeAll<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider == null || !slider.gameObject.scene.IsValid())
                continue;

            if (slider.name == "MasterSlider")
                masterSlider = slider;
            else if (slider.name == "MusicSlider")
                musicSlider = slider;
            else if (slider.name == "SFXSlider")
                sfxSlider = slider;
        }
    }

    void BindSlider(Slider slider, float savedValue, UnityEngine.Events.UnityAction<float> onChanged)
    {
        if (slider == null)
            return;

        slider.SetValueWithoutNotify(Mathf.Clamp01(savedValue));
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(onChanged);
    }

    void SetVolume(string prefsKey, string mixerParameter, float value)
    {
        float clampedValue = Mathf.Clamp01(value);
        audioMixer.SetFloat(mixerParameter, LinearToDb(clampedValue));
        PlayerPrefs.SetFloat(prefsKey, clampedValue);
        PlayerPrefs.Save();
    }

    float LinearToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
