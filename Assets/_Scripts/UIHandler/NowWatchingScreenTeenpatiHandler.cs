using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TP
{

    public class NowWatchingScreenTeenpatiHandler : UIHandler
    {

        public override void HideMe()
        {
            //MasterAudioController.instance.PlayAudio(AudioEnum.BUTTONCLICKCLOSE);
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
        }
    }
}