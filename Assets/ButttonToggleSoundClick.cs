using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButttonToggleSoundClick : MonoBehaviour
{
    public static ButttonToggleSoundClick Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayButtonClick()
    {
        Debug.Log("AUDIO SOUND ============>" + MasterAudioController.instance.CheckSoundToggle());
        if (MasterAudioController.instance.CheckSoundToggle())
        {
            Debug.Log("AUDIO SOUND ============>");
            MasterAudioController.instance.StopAudio(AudioEnum.BUTTONCLICK);
            MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
        }

    }

    public void PlayToggleClick()
    {
        if (MasterAudioController.instance.CheckSoundToggle())
        {
            MasterAudioController.instance.StopAudio(AudioEnum.TOGGLECLICK);
            MasterAudioController.instance.PlayAudio(AudioEnum.TOGGLECLICK);
        }

        

    }
}
