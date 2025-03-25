using DG.Tweening;
using TP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPanelHandler : MonoBehaviour
{
    public RectTransform PanelTransform;
    public float XOffPos = 630;
    public Button MenuButton;
    public Button ExitBtn;
    public Button wholeExit;
    public Button fullScreen;
    public Button HowToPlay;
    public Toggle SoundToggle;
    public Toggle MusicToggle;
    private bool MusicActive;
    public GameObject HowToPlayObj;
    private bool SoundActive;
    public UnityAction SwapSpriteSequence;
    public TMP_Text playerNameTxt;
    public TMP_Text SoundTXT;
    public TMP_Text MusicTXT;


    public GameObject EnableSettingsWholeButton;
    public GameObject Sound;
    public GameObject Music;
    public Sprite SoundRed;
    public Sprite SoundWhite;
    public Sprite MusicRed;
    public Sprite MusicWhite;
    private void Awake()
    {

        SoundToggle.onValueChanged.RemoveAllListeners();
        SoundToggle.onValueChanged.AddListener(ToggleSound);
        MusicToggle.onValueChanged.RemoveAllListeners();
        MusicToggle.onValueChanged.AddListener(ToggleMusic);
        ExitBtn.onClick.RemoveAllListeners();
        ExitBtn.onClick.AddListener(OnExitBtnClick);
        wholeExit.onClick.RemoveAllListeners();
        wholeExit.onClick.AddListener(HideSettings);
        fullScreen.onClick.RemoveAllListeners();
        fullScreen.onClick.AddListener(FullScreen);
        HowToPlay.onClick.RemoveAllListeners();
        HowToPlay.onClick.AddListener(HowToPlayOpen);
    }


    private void Start()
    {
        MenuButton.interactable = true;
        //ToggleSound(true);
    }

    public void CheckPlayerprefs()
    {
        //if (APIController.instance.userDetails.isBlockApiConnection)
        //{
        //    //ToggleSound(PlayerPrefs.GetInt("SoundActive", 1) == 1);
        //    //ToggleMusic(PlayerPrefs.GetInt("ToggleMusic", 1) == 1);


        //}
        //else
        //{
        //    ToggleSound(APIController.instance.authentication.sound);
        //    ToggleMusic(APIController.instance.authentication.music);
        //}

        string sounds = LocalStorage.Load("Lootrix_sound", "1");
        string musics = LocalStorage.Load("Lootrix_music", "1");

        if (string.IsNullOrEmpty(sounds) && string.IsNullOrEmpty(musics))
        {
            ToggleSound(true);
            ToggleMusic(true);
        }
        else
        {
            ToggleSound(LocalStorage.Load("Lootrix_sound", "1") == "1");
            ToggleMusic(LocalStorage.Load("Lootrix_music", "1") == "1");
        }
    }

    public void ToggleSound(bool value)
    {
        DebugHelper.Log("============> " + value);
        APIController.instance.authentication.sound = value;
        CancelInvoke(nameof(UpdateMusic));
        Invoke(nameof(UpdateMusic), 0.5f);
        SoundActive = value;
        SoundToggle.isOn = value;
        LocalStorage.Save("Lootrix_sound", SoundActive ? "1" : "0");

        // PlayerPrefs.SetInt("SoundActive", SoundActive ? 1 : 0);
        if (!value)
        {
            //Sound.GetComponent<Image>().sprite = SoundRed;
            VertexGradient gradient = new VertexGradient(
         new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
         new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
         new Color32(207, 40, 41, 255),   // Bottom color: #CF2829FF
         new Color32(207, 40, 41, 255));  // Bottom color: #CF2829FF


            SoundTXT.colorGradient = gradient;
            
        }
        else
        {
            Sound.GetComponent<Image>().sprite = SoundWhite;

            VertexGradient gradient = new VertexGradient(
            new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
            new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
            new Color32(40, 127, 77, 255),   // Bottom color: #287F4DFF
            new Color32(40, 127, 77, 255)    // Bottom color: #287F4DFF
     );


            SoundTXT.colorGradient = gradient;
            
        }
#if !UNITY_SERVER
            MasterAudioController.instance.CheckSoundToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif

    }

    public void ToggleMusic(bool value)
    {
        DebugHelper.Log("============> " + value);
        APIController.instance.authentication.music = value;
        CancelInvoke(nameof(UpdateMusic));
        Invoke(nameof(UpdateMusic), 0.5f);
        MusicActive = value;
        MusicToggle.isOn = value;

        LocalStorage.Save("Lootrix_music", MusicActive ? "1" : "0");
        // PlayerPrefs.SetInt("ToggleMusic", MusicActive ? 1 : 0);
        /*if (value)
        {
            Music.GetComponent<Image>().sprite = MusicRed;
        }
        else
        {
            Music.GetComponent<Image>().sprite = MusicWhite;

        }*/

        if (value)
        {

            Sound.GetComponent<Image>().sprite = SoundRed;
            
            VertexGradient gradient = new VertexGradient(
           new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
           new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
           new Color32(40, 127, 77, 255),   // Bottom color: #287F4DFF
           new Color32(40, 127, 77, 255));    // Bottom color: #287F4DFF
            MusicTXT.colorGradient = gradient;


        }
        else
        {

            Sound.GetComponent<Image>().sprite = SoundWhite;

             VertexGradient gradient = new VertexGradient(
         new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
         new Color32(255, 255, 255, 255), // Top color: #FFFFFFFF
         new Color32(207, 40, 41, 255),   // Bottom color: #CF2829FF
         new Color32(207, 40, 41, 255));  // Bottom color: #CF2829FF


            MusicTXT.colorGradient = gradient;

        }



#if !UNITY_SERVER
        MasterAudioController.instance.CheckMusicToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif

    }

    public void ShowSettings()
    {
        PanelTransform?.DOKill();
        PanelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-533, -838, 0);
        gameObject.SetActive(true);
        PanelTransform.gameObject.SetActive(true);
        MenuButton.interactable = false;
        playerNameTxt.text = APIController.instance.userDetails.name;
        wholeExit.gameObject.SetActive(true);
        PanelTransform.GetComponent<CanvasGroup>().DOFade(1, 0.1f).OnComplete(() => { MenuButton.interactable = true; });
        PanelTransform.DOAnchorPosX(570, 0.1f);
    }
    public void HideSettings()
    {
        PanelTransform?.DOKill();
        PanelTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(630, -838, 0);
        MenuButton.interactable = false;
        wholeExit.gameObject.SetActive(false);
        PanelTransform.DOAnchorPosX(-533, 0.3f);
        PanelTransform.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => { gameObject.SetActive(false); PanelTransform.gameObject.SetActive(false); MenuButton.interactable = true; });
    }

    public void OnExitBtnClick()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((success) =>
        {
            if (success)
            {
                GameController.Instance.isInGame = false;
                GamePlayUI.instance.ExitGame();
                HideSettings();
                DebugHelper.Log("Exit Called Success");
            }
            else
            {
                if (!UIController.Instance.InternetPopNew.activeSelf)
                    UIController.Instance.InternetPopNew.SetActive(true);
            }
        });
#endif

    }

    void UpdateMusic()
    {
#if !UNITY_SERVER
        APIController.instance.CheckInternetForButtonClick((success) =>
        {
            if (success)
            {
                APIController.instance.UpdateAudioSettings();
            }
            else
            {
                Invoke(nameof(UpdateMusic), 0.5f);
            }
        });
#endif
    }

    public void HowToPlayOpen()
    {
        if (MasterAudioController.instance.CheckSoundToggle())
            MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
        DebugHelper.Log("How  to Play 1");
        wholeExit.gameObject.SetActive(false);
        DebugHelper.Log("How  to Play 2");
        PanelTransform.anchoredPosition = new Vector2(-533f, -714f);
        DebugHelper.Log("How  to Play 3");
        gameObject.SetActive(false);
        DebugHelper.Log("How  to Play 4");
        PanelTransform.gameObject.SetActive(false);
        DebugHelper.Log("How  to Play 5");
        if (MasterAudioController.instance.CheckSoundToggle())
            MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
    }

    public void FullScreen()
    {
#if UNITY_WEBGL
        APIController.FullScreen();
#endif
        HideSettings();
    }
}
