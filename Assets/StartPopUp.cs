using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
namespace TP
{

    public class StartPopUp : MonoBehaviour
    {
        public static StartPopUp Instance;
        public TMP_Text val;
        public Toggle SoundToggle;
        public Toggle MusicToggle;
        private bool SoundActive;
        private bool MusicActive;
        public Button AcceptButton;
        public Button CancelButton;
        public Button CloseButton;
        private void Awake()
        {

            Instance = this;
            AcceptButton.onClick.AddListener(() => { AcceptButtonClick(); });
            CancelButton.onClick.AddListener(() => { RejectButtonClick(); });
            CloseButton.onClick.AddListener(() => { RejectButtonClick(); });
            SoundToggle.onValueChanged.RemoveAllListeners();
            SoundToggle.onValueChanged.AddListener(ToggleSound);
            MusicToggle.onValueChanged.RemoveAllListeners();
            MusicToggle.onValueChanged.AddListener(ToggleMusic);
        }

        private void OnEnable()
        {
            Debug.Log("CHECK START ***********");
            CheckPlayerprefs();
            try
            {
                val.text =   "10.00 " + APIController.instance.userDetails.currency_type + " will be taken as the entry fee for betting in this round.";
            }catch
            {

            }
          
        }

        private void Start()
        {
            PlayerPrefs.DeleteKey("ToggleMusic");
            CheckPlayerprefs();
        }
        void CheckPlayerprefs()
        {
            Debug.Log(PlayerPrefs.GetInt("ToggleMusic", 1) + " ************************");
            ToggleSound(PlayerPrefs.GetInt("SoundActive", 1) == 1);
            ToggleMusic(PlayerPrefs.GetInt("ToggleMusic", 1) == 1);
        }

        public void ToggleSound(bool value)
        {
            SoundActive = value;
            SoundToggle.isOn = value; 
            GamePlayUI.instance.settingsPanel.SoundToggle.isOn = value;
            PlayerPrefs.SetInt("SoundActive", SoundActive ? 1 : 0);
#if !UNITY_SERVER
            MasterAudioController.instance.CheckSoundToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif


        }

        public void ToggleMusic(bool value)
        {
            Debug.Log(value + " =================> ");
            MusicActive = value;
            MusicToggle.isOn = value;
            GamePlayUI.instance.settingsPanel.MusicToggle.isOn = value;
            PlayerPrefs.SetInt("ToggleMusic", MusicActive ? 1 : 0);
#if !UNITY_SERVER
            MasterAudioController.instance.CheckMusicToggle(value && APIController.instance.isOnline && APIController.instance.isInFocus);
#endif
        }

        public void SetVolumeOn()
        {
            Debug.Log("AUDIO SOUND ============>");
            GameController.Instance.isAudioPlayStarted = true;
            AudioListener.volume = 1;
            if (SoundToggle.isOn)
                  MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);

        }

       

      

        public void ExitGame()
        {
            Invoke(nameof(DelayExit), 0.5f);
        }

        public void DelayExit()
        {
#if UNITY_WEBGL
            APIController.CloseWindow();
#endif
        }

        public void AcceptButtonClick()
        {
#if !UNITY_SERVER
            APIController.instance.CheckInternetForButtonClick((Success) =>
            {

                if(Success)
                {
                    if (PlayerManager.localPlayer != null)
                    {
                        SetVolumeOn();
                        GameController.Instance.StartGameOnButtonClick();
                        this.gameObject.SetActive(false);
                    }
                    else
                    {
                        UIController.Instance.FindGameWEBGL();
                        GameController.Instance.SearchOnInternetCheck = true;
                        this.gameObject.SetActive(false);
                    }


                }
                else
                {

                }


            });
#endif

        }

        public void RejectButtonClick()
        {

#if !UNITY_SERVER
            APIController.instance.CheckInternetForButtonClick((Success) =>
            {

                if (Success)
                {
                    ExitGame();

                }
                else
                {
         
                }


            });

#endif
        }

    }
}
