using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using SimpleJSON;

namespace TP
{

    public class PlayerProfileInfoTeenpattiHandler : UIHandler
    {

        public PlayerUI[] players;
        public Image playerProfilePic;
        public Image playerProfilePicSmall;
        public TMP_Text playerName;
        public TMP_Text playerIdTxt;
        public TMP_Text playerLevelTxt;
        public TMP_Text playerFriendsCountTxt;
        public Text tableChipsWonTxt;
        public TMP_Text tableChipsWon;
        public Text tableHandsWonTxt;
        public TMP_Text handsWonLastWeekTxt;
        public Button sendAddFriendBtn;
        public Button sendGiftBtn;
        public Button sendMsgBtn;
        public Button blockUserBtn;
        public UIHandler invitePlayers;
        public TMP_Text Level;
        public TMP_Text Friends;
        public Image Coin;
        public Sprite[] pic;
        public GameObject[] starUI;
        public TMP_Text Chips;


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
            // Coin.sprite = pic[0];
            Coin.sprite = GameController.Instance.CurrentAmountType == CashType.CASH ? pic[1] : pic[0];
            /*Chips.text = GameController.Instance.CurrentAmountType == CashType.CASH ? CommonFunctions.Instance.GetAmountDecimalSeparator( GameController.Instance.CurrentPlayerData.GetGoldVal()) : CommonFunctions.Instance.GetAmountDecimalSeparator(GameController.Instance.CurrentPlayerData.GetSilverVal());
            Friends.text = GameController.Instance.playerNonFriendsList.Count().ToString()+ "" + "/10";
            Level.text =  "Level"+ " " + GameController.Instance.CurrentPlayerData.GetPlayerLevel().ToString();
            handsWonLastWeekTxt.text = GameController.Instance.CurrentPlayerData.GetWeeklyHandsWonVal().ToString();*/
            //FetchUpdatedFriendList();
          
        }






        public void UpdatePlayerProfileInfo(int index)
        {

            if (GameController.Instance.CurrentAmountType == CashType.CASH)
                tableChipsWon.text = "Amount Won :";
            else
                tableChipsWon.text = "Chips Won :";

            playerProfilePic.sprite = players[index].playerAvatar.sprite;
            playerProfilePicSmall.sprite = players[index].playerAvatar.sprite;
            playerName.text = players[index].myPlayerState.playerData.playerName.ToString();
            playerIdTxt.text = players[index].myPlayerState.playerData.playerID;
            Chips.text = players[index].myPlayerState.playerData.silver.ToString().Replace("-", "");
            Friends.text = players[index].myPlayerState.playerData.playerFriendsCount + "/10";
            Level.text = "Level " + players[index].myPlayerState.playerData.playerLevel;
            handsWonLastWeekTxt.text = players[index].myPlayerState.playerData.handsWonCount.ToString();
            tableChipsWonTxt.text = players[index].myPlayerState.playerData.currentTableChipsWon.ToString();
            tableHandsWonTxt.text = players[index].myPlayerState.playerData.currentTableGamesWon.ToString();
            /*switch (index)
            {
                

                    playerProfilePic.sprite = players[0].playerAvatar.sprite;
                   playerProfilePicSmall.sprite = players[0].playerAvatar.sprite;
                    playerName.text = players[0].myPlayerState.playerData.playerName.ToString();
              
                    break;
                case 1:
                    playerProfilePic.sprite = players[1].playerAvatar.sprite;
                    playerProfilePicSmall.sprite = players[1].playerAvatar.sprite;
                    playerName.text = players[1].myPlayerState.playerData.playerName.ToString();
           
                    break;
                case 2:
                    playerProfilePic.sprite = players[2].playerAvatar.sprite;
                    playerProfilePicSmall.sprite = players[2].playerAvatar.sprite;
                    playerName.text = players[2].myPlayerState.playerData.playerName.ToString();

                    break;
                case 3:
                    playerProfilePic.sprite = players[3].playerAvatar.sprite;
                    playerProfilePicSmall.sprite = players[3].playerAvatar.sprite;
                    playerName.text = players[3].myPlayerState.playerData.playerName.ToString();

                    break;
                case 4:
                    playerProfilePic.sprite = players[4].playerAvatar.sprite;
                    playerProfilePicSmall.sprite = players[4].playerAvatar.sprite;
                    playerName.text = players[4].myPlayerState.playerData.playerName.ToString();

                    break;
            }*/
            Debug.Log("run");

            var isGuestPlayer = players[index].myPlayerState.playerData.playerName.StartsWith("Guest");
            if (isGuestPlayer)
            {
                sendAddFriendBtn.interactable = false;
                Debug.LogError("Guest");
            }
            else
            {
                //bool isFriendAlready = GameController.Instance.playerFriendsList.Any(x => string.Equals(x.FriendPlayFabId, players[index].myPlayerState.playerData.playerID));
             
              
               
            }

            sendGiftBtn.onClick.AddListener(() =>
            {
            });

            //if(GameController.Instance.CurrentPlayerData.GetPlayfabID() == players[index].playerID)
            //{
            //    sendMsgBtn.gameObject.SetActive(false);

            //}




            sendMsgBtn.onClick.AddListener(() =>
            {
       //        UIController.Instance.chatHandler.ShowMe();
             
                ChatController.Instance.isPrivateChat = true;
                HideMe();
            });
            blockUserBtn.onClick.AddListener(() =>
            {
            });

        }

        void AddFrinds(string playerId)
        {
            PlayerManager.localPlayer.AddFriend(playerId);
        }

        public void FetchUpdatedFriendList()
        {
         

        }


    }
}




















