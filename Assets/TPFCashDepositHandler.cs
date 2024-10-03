using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{



public class TPFCashDepositHandler : UIHandler
{
        public override void HideMe()
        {
            UIController.Instance.RemoveFromOpenPages(this);
            this.gameObject.SetActive(false);
        }

        public override void OnBack()
        {
            HideMe();
        }

        public override void ShowMe()
        {

            UIController.Instance.AddToOpenPages(this);
            this.gameObject.SetActive(true);
        }
    }
}
