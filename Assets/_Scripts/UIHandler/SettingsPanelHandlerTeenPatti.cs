using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
namespace TP
{
    public class SettingsPanelHandlerTeenPatti : UIHandler
    {
        public Button gulakGold;
        public RectTransform Bg;
        [Header("*******Toggle Button Variables******")]
        [SerializeField]
        private Toggle _toggleSound;
        [SerializeField]
        private Toggle _toggleVibrate;
        public Toggle _toggleGullakGold;
        public Toggle _toggleHandStrengthMeter;
        public TMP_Text fbButtonText;
        public TMP_Text playFabID;
        public override void HideMe()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICKCLOSE);
            UIController.Instance.RemoveFromOpenPages(this);

            Bg.DOAnchorPosX(-904, 0.8f).OnComplete(() =>
            {
                gameObject.SetActive(false);
              
            });
#if TPV
            Bg.DOAnchorPosX(-700, 0.8f).OnComplete(() =>
            {
                gameObject.SetActive(false);

            });
#endif
        }

        public override void OnBack()
        {
            HideMe();
        }


        private void Awake()
        {
            
        }



        public override void ShowMe()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
            UIController.Instance.AddToOpenPages(this);
            gameObject.SetActive(true);
      
            Bg.DOAnchorPosX(884, 0.8f).OnStart(() =>
            {
                
            });
#if TPV
            Bg.DOAnchorPosX(600, 0.8f).OnStart(() =>
            {

            });
#endif
            // _toggleGullakGold.gameObject.SetActive(GameController.Instance.CurrentPlayerData.IsFBLoggedIn() ? true :false);
            HandstrengthMeter();
            fbButtonText.text = GameController.Instance.CurrentPlayerData.IsFBLoggedInLinked() ?  "Logout": "Connect" ;
            //if (GameController.Instance.CurrentPlayerData.IsFBLoggedIn())
            //{
            //    _toggleGullakGold.gameObject.SetActive(CommonFunctions.Instance.GetSavedGullakGoldPref() == OnOffOption.ON ? true : false);
            //}


          //  _toggleGullakGold.gameObject.SetActive(CommonFunctions.Instance.GetSavedGullakGoldPref() == OnOffOption.ON ? true : false);
            playFabID.text = "Player ID:" + " " + GameController.Instance.CurrentPlayerData.GetPlayfabID().ToString();

        }


        public void HandstrengthMeter()
        {
            if (GameController.Instance.CurrentPlayerData.IsFBLoggedInLinked())
            {
               _toggleHandStrengthMeter.gameObject.SetActive(true);
            }
            else
            {
                _toggleHandStrengthMeter.gameObject.SetActive(true);
                //if (GameController.Instance.HandstrengthMeterFreeDays > 15 || GameController.Instance.HandstrengthMeterFreeDays < 1)
                //    _toggleHandStrengthMeter.gameObject.SetActive(false);
            }

        }

        public void OnclickFbButton()
        {
            switch (GameController.Instance.CurrentPlayerData.IsFBLoggedInLinked())
            {
                case true:
              //      UIController.Instance.FacebookLogoutPanel.ShowMe();
                    break;
                default:
              //      UIController.Instance.FacebookLoginPanel.ShowMe();
                    break;
            }
        }

       
    }
}




















