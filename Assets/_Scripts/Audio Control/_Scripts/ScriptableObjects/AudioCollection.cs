using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioData", menuName = "SwipeWire/AudioData", order = 1)]
public class AudioCollection : ScriptableObject
{
    public List<AudioDate> audioData = new List<AudioDate>();
}

[System.Serializable]
public struct AudioDate
{
    public AudioClip audioClip;
    public AudioEnum audioName;
    [Range(0,1)]
    public float volume;
    [Range(0,5)]
    public float pitch;
    public bool mute;
}

public enum AudioEnum
{
    BG,
    BUTTONCLICK,
    BUTTONCLICKCLOSE,
    CHIPSOUND,
    CARDDEALTEENPATTI,
    CHALTEENPATTI,
    WINNERTEENPATTI,
    CARDDEALRUMMY,
    CARDDEALFLIPRUMMY,
    WINNERRUMMY,
    CARDSLIDERUMMY,
    TIMER,
    TIMERANTHARBAHAR,
    DEALCARDSANTHARBAHAR,
    TURN,
    PROGRESS,
    TOGGLECLICK,
    FOLD,
    PACK,
    COUNTDOWN

}