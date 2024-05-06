
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenuController : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;

    private string musicVolume = "MusicVolume";
    private string sfxVolume = "SfxVolume";

    private void Awake()
    {
        if(PlayerPrefs.HasKey(musicVolume))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSfxVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat(musicVolume, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(musicVolume, volume);
    }

    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat(sfxVolume, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(sfxVolume, volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat(musicVolume);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxVolume);

        SetMusicVolume();
        SetSfxVolume();
    }
}
