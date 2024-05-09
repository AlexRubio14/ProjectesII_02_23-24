using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioVolumeManager : MonoBehaviour
{
    public static AudioVolumeManager instance;

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private string musicMixerParameter;
    [SerializeField]
    private string sfxMixerParameter;

    [Space, SerializeField]
    private string musicKey;
    [SerializeField]
    private float musicDefaultValue;
    public float musicValue { get; private set; }

    [Space, SerializeField]
    private string sfxKey;
    [SerializeField]
    private float sfxDefaultValue;
    public float sfxValue { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadAudioVolumes();
    }

    private void LoadAudioVolumes()
    {
        if (!PlayerPrefs.HasKey(musicKey))
            SaveMusicValue(musicDefaultValue);
        else
            SaveMusicValue(PlayerPrefs.GetFloat(musicKey));


        if (!PlayerPrefs.HasKey(sfxKey))
            SaveSfxValue(sfxDefaultValue);
        else
            SaveSfxValue(PlayerPrefs.GetFloat(sfxKey));

    }

    public void SaveMusicValue(float _value)
    {
        PlayerPrefs.SetFloat(musicKey, _value);
        audioMixer.SetFloat(musicMixerParameter, Mathf.Log10(_value) * 20);
        musicValue = _value;
    }
    public void SaveSfxValue(float _value)
    {
        PlayerPrefs.SetFloat(sfxKey, _value);
        audioMixer.SetFloat(sfxMixerParameter, Mathf.Log10(_value) * 20);
        sfxValue = _value;
    }

}
