using System.Collections;
using System.Collections.Generic;
using TP;
using UnityEngine;

public class MasterAudioController : MonoBehaviour
{

    public static MasterAudioController instance;
    [SerializeField] bool muteAllAudio;

  public Queue<AudioSource> audioSources = new Queue<AudioSource>();

    [System.Serializable]
    public class AudioByType
    {
        public AudioEnum audioEnum;        
        public List<AudioSource> audioActive;
    }

   public  List<AudioByType> audiosActive = new List<AudioByType>();
    
    int poolSize = 15;

  public  AudioCollection audioCollection;

    private void Awake()
    {
        instance = this;
        audioCollection = Resources.Load("AudioData", typeof(AudioCollection)) as AudioCollection;
        InitializePool();
    }

    void Start()
    {
        
    }
    public void CheckSoundToggle(bool isMute)
    {

        DebugHelper.Log("CheckSoundToggle==================> " + isMute);
        foreach (var audioType in audiosActive.FindAll(x => x.audioEnum != AudioEnum.BG))
        {
            if (audioType.audioActive.Count > 0)
            {
                foreach (AudioSource source in audioType.audioActive)
                {
                    source.mute = !isMute;
                }
            }
        }
    }

    public void CheckMusicToggle(bool isMute)
    {
        DebugHelper.Log("CheckMusicToggle==================> " + isMute);
        foreach (var audioType in audiosActive.FindAll(x => x.audioEnum == AudioEnum.BG))
        {
            if (audioType.audioActive.Count > 0)
            {
                foreach (AudioSource source in audioType.audioActive)
                {
                    source.mute = !isMute;
                }
            }
        }
    }





    public void StopAudio(AudioEnum audioToPlay)
    {
        AudioByType audiotypeTostop = audiosActive.Find(x => x.audioEnum == audioToPlay);
        if (audiotypeTostop == null) return;
        foreach (var item in audiotypeTostop.audioActive)
        {
            item.Stop();
            audioSources.Enqueue(item);
        }
        audiosActive.Remove(audiotypeTostop);
    }    

    public void PlayAudio(AudioEnum audioToPlay, bool loop = false)
    {
        DebugHelper.Log("Check Sound Play");
        //========================================================================== TEMP
        if (audioToPlay == AudioEnum.BUTTONCLICKCLOSE)
        {
            AudioByType TYPE = audiosActive.Find(x => x.audioEnum == AudioEnum.BUTTONCLICKCLOSE);
            if(TYPE !=null)
            if (TYPE.audioActive.Count > 0)
                return;
        }
        //==================================================================
        if (muteAllAudio) return;
        AudioDate collection = audioCollection.audioData.Find(x => x.audioName == audioToPlay);
        if (collection.mute) return;
        AudioSource source = GetAudioSource();
        source.clip = collection.audioClip;
        source.loop = loop;
        source.volume = collection.volume;
        source.pitch = collection.pitch;
        source.Play();

        AudioByType audioType = audiosActive.Find(x => x.audioEnum == audioToPlay);
        if (audioType != null)
            audioType.audioActive.Add(source);
        else
            audiosActive.Add(new AudioByType { audioEnum = audioToPlay, audioActive = new List<AudioSource> { source } });
        if (!loop)
            StartCoroutine(EnqueueAudioSource(source));
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    void CreateNewAudioSource()
    {
        GameObject go = new GameObject("Audio", typeof(AudioSource));
        go.hideFlags = HideFlags.HideAndDontSave;
        go.transform.SetParent(transform);
        AudioSource audiosource = go.GetComponent<AudioSource>();
        audiosource.loop = false;
        audiosource.volume = 1;
        audiosource.playOnAwake = false;
        audioSources.Enqueue(audiosource);

    }

 public  AudioSource GetAudioSource()
    {
        AudioSource audioSource = null;
        if(audioSources.Count == 0)
        {
            CreateNewAudioSource();
        }
        audioSource = audioSources.Dequeue();
        return audioSource;
    }

    IEnumerator EnqueueAudioSource(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        if (source.clip != null)
        {
            source.Stop();
            audioSources.Enqueue(source);
            foreach (var audiotype in audiosActive)
            {
                if (audiotype.audioActive.Contains(source))
                {
                    audiotype.audioActive.Remove(source);
                }
            }
        }

    }

    public bool CheckSoundToggle()
    {
        return GamePlayUI.instance.settingsPanel.SoundToggle.isOn; /*|| StartPopUp.Instance.SoundToggle.isOn;*/
    }

    public bool CheckMusicToggle()
    {
        return GamePlayUI.instance.settingsPanel.MusicToggle.isOn; /*|| StartPopUp.Instance.MusicToggle.isOn;*/
    }
}
