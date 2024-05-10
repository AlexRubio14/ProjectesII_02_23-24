
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

    private void OnEnable()
    {
        LoadVolume();
    }
    public void UpdateMusicVolume()
    {
        float volume = (musicSlider.value / 10) + 0.0001f;
        AudioVolumeManager.instance.SaveMusicValue(volume);
    }

    public void UpdateSfxVolume()  
    {
        float volume = (sfxSlider.value / 10) + 0.0001f;
        AudioVolumeManager.instance.SaveSfxValue(volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = AudioVolumeManager.instance.musicValue * 10;
        sfxSlider.value = AudioVolumeManager.instance.sfxValue * 10;
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
