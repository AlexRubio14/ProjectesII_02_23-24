using UnityEngine;

public class UiButtonHandler : MonoBehaviour
{
    [SerializeField]
    private AudioClip clickClip;
    [SerializeField]
    private AudioClip hoverClip;

    public void PlayOnClickClip()
    {
        AudioManager.instance.Play2dOneShotSound(clickClip, "Button");
        GamepadRumbleManager.Instance.AddRumble(new GamepadRumbleManager.Rumble(0f, 0.05f, 0.15f, false));
    }

    public void PlayOnHoverClip()
    {
        AudioManager.instance.Play2dOneShotSound(hoverClip, "Button");
    }

  
}
