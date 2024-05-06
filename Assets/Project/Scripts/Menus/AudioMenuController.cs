
using System.Collections.Generic;
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

    

    [SerializeField]
    private List<Button> buttonList;
    private void Awake()
    {
        if(PlayerPrefs.HasKey(AudioManager.instance.musicVolume))
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
        audioMixer.SetFloat(AudioManager.instance.musicVolume, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(AudioManager.instance.musicVolume, volume);
    }

    public void SetSfxVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat(AudioManager.instance.sfxVolume, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(AudioManager.instance.sfxVolume, volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.musicVolume);
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.sfxVolume);

        SetMusicVolume();
        SetSfxVolume();
    }

    public void DeactiveButtons()
    {
        foreach (Button bt in buttonList)
            bt.interactable = false;
    }

    public void ActivateButtons()
    {
        foreach (Button bt in buttonList)
            bt.interactable = true;
    }
}
