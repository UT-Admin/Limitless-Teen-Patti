using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TP
{
    public class ChipDepositParentHandler : UIHandler
    {
        public Toggle Cash;
        public Toggle Chip;
        public UIHandler cashPanel, chipPanel;
        public GameObject ChipHolder;
        


        public override void HideMe()
        {
#if TPF
#else
            UIController.Instance.RemoveFromOpenPages(this);
#endif
            this.gameObject.SetActive(false);

        }

        public override void OnBack()
        {
#if TPF
#else
            HideMe();
#endif
        }
        

        public override void ShowMe()
        {
#if TPF
#else
            UIController.Instance.AddToOpenPages(this);
#endif
            this.gameObject.SetActive(true);
            ChipHolder.gameObject.SetActive(Chip.isOn ? true : false);
        }

        public void OnClickToggleCash()
        {
            if (Cash.isOn)
            {
                cashPanel.ShowMe();
               
            }
            else
            {
                cashPanel.HideMe();
                
            }
        }

        public void OnClickToggleChip()
        {
            if (Chip.isOn)
            {
                chipPanel.ShowMe();
                ChipHolder.SetActive(true);
            }
            else
            {
                chipPanel.HideMe();
                ChipHolder.SetActive(false);
            }
        }
    }
}