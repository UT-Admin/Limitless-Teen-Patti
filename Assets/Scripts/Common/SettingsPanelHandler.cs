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
    public float XOffPos = 700;
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
    public GameObject EnableSettingsWholeButton;
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
        //ToggleSound(true);
    }

    void CheckPlayerprefs()
    {
        ToggleSound(PlayerPrefs.GetInt("SoundActive", 1) == 1);
        ToggleMusic(PlayerPrefs.GetInt("ToggleMusic", 1) == 1);
    }

    public void ToggleSound(bool value)
    {
        Debug.Log("============> " + value);
        SoundActive = value;
        SoundToggle.isOn = value;
        PlayerPrefs.SetInt("SoundActive", SoundActive ? 1 : 0);
#if !UNITY_SERVER
        MasterAudioController.instance.CheckSoundToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif

    }

    public void ToggleMusic(bool value)
    {
        Debug.Log("============> " + value);
        MusicActive = value;
        MusicToggle.isOn = value;
        PlayerPrefs.SetInt("ToggleMusic", MusicActive ? 1 : 0);
#if !UNITY_SERVER
        MasterAudioController.instance.CheckMusicToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif

    }

    public void ShowSettings()
    {
        playerNameTxt.text = APIController.instance.userDetails.name;
        CheckPlayerprefs();
        gameObject.SetActive(true);
        wholeExit.gameObject.SetActive(true);
        PanelTransform?.DOKill();
        PanelTransform.GetComponent<CanvasGroup>().DOFade(1, 0.1f);
        PanelTransform.gameObject.SetActive(true);
        PanelTransform.DOAnchorPosX(640, 0.1f);
    }
    public void HideSettings()
    {
        wholeExit.gameObject.SetActive(false);
        PanelTransform?.DOKill();
        PanelTransform.DOAnchorPosX(-533, 0.3f);
        PanelTransform.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => { gameObject.SetActive(false); PanelTransform.gameObject.SetActive(false); });
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

    public void HowToPlayOpen()
    {
        if (MasterAudioController.instance.CheckSoundToggle())
            MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
        Debug.Log("How  to Play 1");
        wholeExit.gameObject.SetActive(false);
        Debug.Log("How  to Play 2");
        PanelTransform.anchoredPosition = new Vector2(-533f, -714f);
        Debug.Log("How  to Play 3");
        gameObject.SetActive(false);
        Debug.Log("How  to Play 4");
        PanelTransform.gameObject.SetActive(false);
        Debug.Log("How  to Play 5");
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
