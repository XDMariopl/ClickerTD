using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicSettingsUI : MonoBehaviour
{
    public TMP_Dropdown musicDropdown;
    public Toggle loopToggle;

    void Start()
    {
        var player = MusicPlayer.Instance;

        // Populate dropdown
        musicDropdown.ClearOptions();
        foreach (var clip in player.tracks)
        {
            musicDropdown.options.Add(
                new TMP_Dropdown.OptionData(clip.name)
            );
        }

        // Load saved values
        musicDropdown.value = player.CurrentIndex();
        musicDropdown.RefreshShownValue();

        loopToggle.isOn = player.IsLooping();

        // Bind events
        musicDropdown.onValueChanged.AddListener(OnMusicChanged);
        loopToggle.onValueChanged.AddListener(OnLoopChanged);
    }

    void OnMusicChanged(int index)
    {
        MusicPlayer.Instance.Play(index, loopToggle.isOn);
    }

    void OnLoopChanged(bool loop)
    {
        MusicPlayer.Instance.Play(musicDropdown.value, loop);
    }
}
