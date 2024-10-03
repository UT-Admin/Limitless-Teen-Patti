using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace TP
{
    public class MenuPopUpTPF : UIHandler
    {
        public Button ExitGame, SwitchTable, StandUp;
        public UIHandler SwitchTablePanel,ExitGamePanel;

        private void Awake()
        {
            ExitGame.onClick.AddListener(() => { OnclickExitGame(); });
            SwitchTable.onClick.AddListener(() => { OnclickSwitchTable(); });
            StandUp.onClick.AddListener(() => { OnclickStandUp(); });
        }

        public override void HideMe()
        {
         
            UIController.Instance.RemoveFromOpenPages(this);
            gameObject.SetActive(false);


        }

        public override void OnBack()
        {
            HideMe();
        }

        public override void ShowMe()
        {
          
            UIController.Instance.AddToOpenPages(this);

            gameObject.SetActive(true);
        }

        public void OnclickExitGame()
        {
            ExitGamePanel.ShowMe();
            HideMe();
        }

        public void OnclickSwitchTable()
        {
            SwitchTablePanel.ShowMe();
            HideMe();
        }

        public void OnclickStandUp()
        {
            HideMe();
        }

    }
}