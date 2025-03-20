using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TP
{
    public class TeenPattiInfoPanelHandler : UIHandler
    {

        [SerializeField] private Image headerImage;
        [SerializeField] private Sprite[] pics;
        [SerializeField] private Sprite[] header;
        public TMP_Text val;

        public override void HideMe()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICKCLOSE);
            UIController.Instance.RemoveFromOpenPages(this);
            gameObject.SetActive(false);
        }

        public override void OnBack()
        {
            HideMe();
        }

        public override void ShowMe()
        {
            if (MasterAudioController.instance.CheckSoundToggle())
                MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICK);
            UIController.Instance.AddToOpenPages(this);
            gameObject.SetActive(true);
            //Image1.sprite = GameController.Instance.CurrentAmountType == CashType.CASH ? pics[3] : pics[0];
            //Image2.sprite = GameController.Instance.CurrentAmountType == CashType.CASH ? pics[4] : pics[1];
            //Image3.sprite = GameController.Instance.CurrentAmountType == CashType.CASH ? pics[5] : pics[2];

#if RealTPG || TPF
            headerImage.sprite = header[(int)GameController.Instance.CurrentGameMode - 1];
            headerImage.SetNativeSize();
#elif GOP



#else
            //Image1.SetNativeSize();
            //Image2.SetNativeSize();
            //Image3.SetNativeSize();

#endif



        }

        private void OnEnable()
        {
            Debug.Log("======================>");
            if (APIController.instance.userDetails.isBlockApiConnection)
            {
                val.text = "";
            }
            else
            {
                val.text = (APIController.instance.userDetails.commission * 100) + "% Commission will be taken from pot amount.";
            }

            
        }
    }
}











