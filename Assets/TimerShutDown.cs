using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerShutDown : MonoBehaviour
{
    public List<GameObject> Objects;

    private void Start()
    {
        PlayAudio = true;
    }
    public void SetObjectoff()
    {
        this.gameObject.SetActive(false);
    }
    public void PlaySound()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            Objects[i].gameObject.SetActive(false);
        }

        if (MasterAudioController.instance.CheckSoundToggle())
            MasterAudioController.instance.PlayAudio(AudioEnum.COUNTDOWN);
    }

    public void stopAudio()
    {
       // MasterAudioController.instance.StopAudio(AudioEnum.COUNTDOWN);
    }

    bool PlayAudio;
    public void PlaymyTurnAudio()
    {
        Debug.Log("TURN SOUND ===============>");

        if (PlayAudio)
        {
            PlayAudio = false;
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.TURN);
        }

    }

    public void ResetAudio()
    {
        //PlayAudio = true;
    }
    public void HideAudio()
    {
        PlayAudio = true;
    }
}
