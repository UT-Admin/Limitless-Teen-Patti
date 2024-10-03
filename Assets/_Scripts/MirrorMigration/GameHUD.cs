using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TP.Mirror
{
    public class GameHUD : MonoBehaviour
    {
        public static GameHUD instance;
        public Text winnerTxt, cardValueTxt, gameStartTimer;
        [SerializeField] GameObject myTurnUI;
        public List<PlayerUI> playerUIDetails = new List<PlayerUI>();
        private void Awake()
        {
            instance = this;
        }

        public void NextPlayerTurn()
        {
            PlayerManager.localPlayer.NextPlayerTurnCommand();
        }

        public void ShowWinner()
        {

        }

        public void ShowHud()
        {
            myTurnUI.SetActive(true);
        }

        public void HideHud()
        {
            //myTurnUI.SetActive(false);
        }

        public void ExitGame()
        {

        }

        public void ClearAllUI()
        {
          
        }

    }
}
