
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
        if(PlayerPrefs.HasKey(AudioManager.instance.musicVolumeString))
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
        float volume = (musicSlider.value / 10) + 0.0001f;
        AudioManager.instance.musicVolume = volume;
        audioMixer.SetFloat(AudioManager.instance.musicVolumeString, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(AudioManager.instance.musicVolumeString, volume);
    }

    public void SetSfxVolume()  
    {
        float volume = (sfxSlider.value / 10) + 0.0001f;
        AudioManager.instance.sfxVolume = volume;
        audioMixer.SetFloat(AudioManager.instance.sfxVolumeString, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(AudioManager.instance.sfxVolumeString, volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.musicVolumeString) * 10;
        sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.instance.sfxVolumeString) * 10;

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

    private void OnDisable()
    {
        SetMusicVolume();
        SetSfxVolume();
    }

    public void SetVolumeAfterErasePlayerPrefs()
    {
        musicSlider.value = AudioManager.instance.musicVolume;
        sfxSlider.value = AudioManager.instance.sfxVolume;

        SetMusicVolume();
        SetSfxVolume();
    }


}
