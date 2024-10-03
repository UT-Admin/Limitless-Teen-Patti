using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachAudioComponent : MonoBehaviour
{
    [SerializeField] AudioEnum audioType;
    [SerializeField] bool loop = false;
    [SerializeField] bool playOnAwake = false;

    private void Start()
    {
        if (playOnAwake)
            PlayAudio();
    }

    [ContextMenu("Test")]
    public void PlayAudio()
    {
        MasterAudioController.instance.PlayAudio(audioType,loop);
    }
}
