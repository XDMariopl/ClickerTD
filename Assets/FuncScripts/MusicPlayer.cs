using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    public AudioSource musicSource;
    public AudioClip[] tracks;

    [Header("Mixer")]
    public AudioMixerGroup musicMixerGroup;

    const string MUSIC_INDEX_KEY = "MusicTrack";
    const string MUSIC_LOOP_KEY = "MusicLoop";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureMusicSource();
        Load();

    }

    void EnsureMusicSource()
    {
        // If we already have a source, just enable it
        if (musicSource != null)
        {
            musicSource.enabled = true;
            musicSource.gameObject.SetActive(true);
            return;
        }

        // Otherwise create a new one
        GameObject go = new GameObject("MusicSource");
        go.transform.SetParent(transform);
        musicSource = go.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        if (musicMixerGroup != null)
            musicSource.outputAudioMixerGroup = musicMixerGroup;
    }


    void OnEnable()
    {
        EnsureMusicSource();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureMusicSource(); // always validate
    }

    void Load()
    {
        int index = PlayerPrefs.GetInt(MUSIC_INDEX_KEY, 0);
        bool loop = PlayerPrefs.GetInt(MUSIC_LOOP_KEY, 1) == 1;

        Play(index, loop);
    }

    public void Play(int index, bool loop)
    {
        EnsureMusicSource();

        if (tracks == null || tracks.Length == 0)
            return;

        index = Mathf.Clamp(index, 0, tracks.Length - 1);

        if (musicSource.clip == tracks[index] && musicSource.isPlaying)
            return; // Already playing this track

        musicSource.Stop();
        musicSource.clip = tracks[index];
        musicSource.loop = loop;
        musicSource.Play(); // Always call Play

        PlayerPrefs.SetInt(MUSIC_INDEX_KEY, index);
        PlayerPrefs.SetInt(MUSIC_LOOP_KEY, loop ? 1 : 0);
    }


    public int CurrentIndex()
    {
        return PlayerPrefs.GetInt(MUSIC_INDEX_KEY, 0);
    }

    public bool IsLooping()
    {
        return PlayerPrefs.GetInt(MUSIC_LOOP_KEY, 1) == 1;
    }
}
